#!/usr/bin/python3

"""
Room placement restrictions:
1. Doors of adjacent rooms should face each other
2. Rooms do not overlap each other
"""

import math
from drawille import Canvas
from random import randint
from enum import Enum
import time
from typing import Set, List, Type, Dict


"""
=======================
     BASE CLASSES
=======================
"""

class TreeNode(object):
    def __init__(self):
        self.parent = None
        self.children: List[TreeNode] = []


class Vector2(object):
    def __init__(self, x: int, y: int):
        self.x = x
        self.y = y

    def __repr__(self):
        return '(%d, %d)' % (self.x, self.y)

    def __add__(self, v):
        return Vector2(self.x + v.x, self.y + v.y)

    def __sub__(self, v):
        return Vector2(self.x - v.x, self.y - v.y)

    def __eq__(self, v):
        if isinstance(v, Vector2):
            return self.x == v.x and self.y == v.y
        return False


class Direction(Enum):
    LEFT = Vector2(-1, 0)
    TOP = Vector2(0, -1)
    RIGHT = Vector2(1, 0)
    BOTTOM = Vector2(0, 1)
    NONE = Vector2(0, 0)

    def __str__(self):
        return self.name


class Door(object):
    def __init__(self, position: Vector2, direction: Direction):
        """
        :param position: door's position in tiles relative to its parent room
        :param direction: the direction the door is facing to
        :type position: Vector2
        :type direction: Direction
        """
        self.position = position
        self.direction = direction

    def opposite(self):
        """
        :return: the opposite door of this door
        :rtype: Door
        """
        return Door(self.position + self.direction.value,
                    Direction(Direction.NONE.value - self.direction.value))

    def __eq__(self, d):
        if isinstance(d, Door):
            return self.position == d.position and self.direction == d.direction
        return False

    def __str__(self):
        return "%s() at %s facing %s" % (self.__class__.__name__, self.position, self.direction)


class Room(TreeNode):

    def __init__(self, size: Vector2, doors: List[Door], position: Vector2 = Vector2(0, 0)):
        """
        :param size: room size in tiles. Hopefully this can be a collider in future design
        :param position: room's world location in tiles
        :param doors: a set of doors this room has
        :type size: Vector2
        :type position: List[Door]
        :type doors: Vector2
        """
        super(Room, self).__init__()
        self.size = size
        self.doors = doors
        self.position = position
        # doors that are connected to other rooms
        self.connected_doors = []
        # doors that connect this room to its parent
        self.parent_link = []

    def contains(self, position):
        """
        :param position: tile position to be tested
        :type position: Vector2
        :return: if the tile position is inside the room
        :rtype: bool
        """
        return self.position.x <= position.x and \
            self.position.x + self.size.x - 1 >= position.x and \
            self.position.y <= position.y and \
            self.position.y + self.size.y - 1 >= position.y

    def is_door_facing_wall(self, room):
        """
        :param room: another room
        :type room: Room
        :return: if any door of this room is connected to a wall of another room
        :rtype: bool
        """
        for door in self.doors:
            opposite_door_position = self.position + door.opposite().position
            if room.contains(opposite_door_position):
                for _door in room.doors:
                    if _door.position + room.position == opposite_door_position:
                        if door.direction.value + _door.direction.value == Direction.NONE.value:
                            return False
                return True
        return False

    def is_overlap(self, room):
        """
        :param room: another room
        :type room: Room
        :return: if another room overlaps with this room
        :rtype: bool
        """
        if self.position.x > room.position.x + room.size.x - 1 or \
                self.position.x + self.size.x - 1 < room.position.x or \
                self.position.y > room.position.y + room.size.y - 1 or \
                self.position.y + self.size.y - 1 < room.position.y:
            if self.is_door_facing_wall(room):
                return True
            if room.is_door_facing_wall(self):
                return True
            return False
        return True

    @property
    def available_doors(self):
        """
        :return: doors that are not connected to a room
        :rtype: Set[Door]
        """
        return [door for door in self.doors if door not in self.connected_doors]

    def can_have_adjacent(self):
        """
        :return: if every door is connected to a room
        :rtype: bool
        """
        return not (len(self.connected_doors) == len(self.doors))

    def can_be_adjacent(self, room):
        """
        :param room: another room
        :type room: Room
        :return: if this room can be adjacent to another room
        :rtype: bool
        """
        return True

    def connect(self, room):
        if not self.can_be_adjacent(room):
            return False
        if not self.can_have_adjacent() or not room.can_have_adjacent():
            return False
        for from_door in self.available_doors:
            for to_door in room.available_doors:
                # if doors are not facing each other, return false
                if from_door.direction.value + to_door.direction.value != Direction.NONE.value:
                    continue
                # move room to a new position where two doors can face each other
                # and test if they overlap
                to_door_position = self.position + from_door.opposite().position
                room.position = to_door_position - to_door.position
                if not self.is_overlap(room):
                    self.connected_doors += [from_door]
                    room.connected_doors += [to_door]
                    room.parent = self
                    room.parent_link = [from_door, to_door]
                    self.children += [room]
                    return True
        return False

    def disconnect(self, room):
        if room not in self.children or room.parent_link[0] not in self.connected_doors:
            return False
        self.connected_doors.remove(room.parent_link[0])
        room.connected_doors.remove(room.parent_link[1])
        room.parent = None
        self.children.remove(room)
        return True


class RoomPlacement(object):
    def __init__(self, room_type: Type[Room], min_amount: int = 1, max_amount: int = math.inf):
        self.room_type = room_type
        self.min_amount = min_amount
        self.max_amount = max_amount


def draw(canvas: Canvas, room: Room):
    _TILE_SIZE = Vector2(5, 5)
    for x in range(room.size.x):
        for y in range(room.size.y):
            # draw walls
            # door directions for this tile
            door_directions = []
            for door in room.doors:
                if Vector2(x, y) == door.position:
                    door_directions += [door.direction]
            for i in range(_TILE_SIZE.x):
                for j in range(_TILE_SIZE.y):
                    if x == 0 and Direction.LEFT not in door_directions:
                        canvas.set((room.position.x + x) * _TILE_SIZE.x,
                                   (room.position.y + y) * _TILE_SIZE.y + j)
                    if x == room.size.x - 1 and Direction.RIGHT not in door_directions:
                        canvas.set((room.position.x + x + 1) * _TILE_SIZE.x,
                                   (room.position.y + y) * _TILE_SIZE.y + j)
                    if y == 0 and Direction.TOP not in door_directions:
                        canvas.set((room.position.x + x) * _TILE_SIZE.x +
                                   i, (room.position.y + y) * _TILE_SIZE.y)
                    if y == room.size.y - 1 and Direction.BOTTOM not in door_directions:
                        canvas.set((room.position.x + x) * _TILE_SIZE.x + i,
                                   (room.position.y + y + 1) * _TILE_SIZE.y)
            for i in range(2, _TILE_SIZE.x - 1):
                for j in range(2, _TILE_SIZE.y - 1):
                    canvas.set((room.position.x + x) * _TILE_SIZE.x + i,
                        (room.position.y + y) * _TILE_SIZE.y + j)


class RoomPlanner(object):
    def __init__(self, start_room: Type[Room], room_placements: List[RoomPlacement]):
        self.room_amount = dict()
        self.room_amount_required = dict()
        self.room_amount_left = dict()
        self.total_min_amount = 0
        self.total_max_amount = 0
        for policy in room_placements:
            self.room_amount[policy.room_type] = 0
            self.room_amount_required[policy.room_type] = policy.min_amount
            self.room_amount_left[policy.room_type] = policy.max_amount
            self.total_min_amount += policy.min_amount
            self.total_max_amount += policy.max_amount
        if self.total_min_amount > self.total_max_amount:
            raise Exception(
                "the minimum amount of rooms must be no smaller than the maximum amount")
        self.cursor = 0
        self.queue = [start_room()]
        self.prev_excluded_types = []

    @property
    def total_amount_chosen(self):
        return sum([self.room_amount[room_type] for room_type in self.room_amount])

    @property
    def total_amount_required(self):
        return sum([self.room_amount_required[room_type] for room_type in self.room_amount_required])

    def choose_room_type(self, pool: Dict[Type[Room], int], excluded_type: List[Type[Room]] = []):
        """
        :param pool: list of available room types and max amount
        :type pool: Dict[Type[Room], int]
        :param excluded_type: list of excluded room types
        :type excluded_type: List[Type[Room]]
        :return: chosen room type
        :rtype: Type[Room]
        """
        i = 0
        types = [room_type for room_type in pool if room_type not in excluded_type]
        total = sum([pool[room_type] for room_type in types])
        if total == 0:
            return None
        r = randint(1, total)
        for room_type in types:
            i += pool[room_type]
            if r <= i:
                return room_type

    def get_excluded_room_type(self, room):
        excluded = []
        for room_type in self.room_amount_left:
            is_excluded = True
            for door in room.available_doors:
                _room = room_type()
                directions = [d.direction for d in _room.doors]
                if door.opposite().direction in directions:
                    is_excluded = False
                    break
            if is_excluded:
                excluded += [room_type]
        return excluded

    def is_overlap(self, next):
        for prev in self.queue:
            if next.is_overlap(prev):
                return True
        return False

    def spawn_next(self):
        prev = self.queue[self.cursor]
        if len(self.prev_excluded_types) == 0:
            self.prev_excluded_types = self.get_excluded_room_type(prev)
        room_type = self.choose_room_type(self.room_amount_left, self.prev_excluded_types)
        if room_type is None:
            self.cursor += 1
            self.prev_excluded_types = []
            return
        next = room_type()
        if prev.connect(next):
            if not self.is_overlap(next):
                self.queue += [next]
                self.room_amount[room_type] += 1
                if self.room_amount_required[room_type] > 0:
                    self.room_amount_required[room_type] -= 1
                if self.room_amount_left[room_type] > 0:
                    self.room_amount_left[room_type] -= 1
            else:
                prev.disconnect(next)
        self.prev_excluded_types += [room_type]
        if len(self.prev_excluded_types) >= len([room_type for room_type in self.room_amount_left]):
            self.cursor += 1
            self.prev_excluded_types = []
        if not prev.can_have_adjacent():
            self.cursor += 1
            self.prev_excluded_types = []

    def spawn(self):
        while(True):
            self.spawn_next()
            print(chr(27) + "[2J")
            _canvas = Canvas()
            for room in self.queue:
                draw(_canvas, room)
            print(_canvas.frame())
            if self.total_amount_required == 0:
                # seal all doors
                for room in self.queue:
                    for door in room.available_doors:
                        room.doors.remove(door)
                print(chr(27) + "[2J")
                _canvas = Canvas()
                for room in self.queue:
                    draw(_canvas, room)
                print(_canvas.frame())
                break
            time.sleep(0.05)


"""
=======================
     Custom Rooms
=======================
"""


class SpawnRoom(Room):
    def __init__(self):
        super(SpawnRoom, self).__init__(Vector2(4, 3),
                                        [Door(Vector2(1, 2), Direction.BOTTOM)])


class BossRoom(Room):
    def __init__(self):
        super(BossRoom, self).__init__(Vector2(3, 4),
                                       [Door(Vector2(1, 0), Direction.TOP)])


class SmallRoomLB(Room):
    def __init__(self):
        super(SmallRoomLB, self).__init__(Vector2(3, 3),
                                          [Door(Vector2(0, 1),
                                                Direction.LEFT),
                                              Door(Vector2(1, 2),
                                                   Direction.BOTTOM),
                                           ])


class MediumRoomTR(Room):
    def __init__(self):
        super(MediumRoomTR, self).__init__(Vector2(6, 4),
                                           [Door(Vector2(3, 0),
                                                 Direction.TOP),
                                               Door(Vector2(5, 2),
                                                    Direction.RIGHT),
                                            ])


class LargeRoomLBR(Room):
    def __init__(self):
        super(LargeRoomLBR, self).__init__(Vector2(8, 8),
                                           [Door(Vector2(0, 2),
                                                 Direction.LEFT),
                                               Door(Vector2(3, 7),
                                                    Direction.BOTTOM),
                                               Door(Vector2(7, 6),
                                                    Direction.RIGHT),
                                            ])

class Connector(Room):
    def __init__(self):
        super(Connector, self).__init__(Vector2(3, 3),
                                           [Door(Vector2(0, 1),
                                                 Direction.LEFT),
                                               Door(Vector2(1, 0),
                                                    Direction.TOP),
                                               Door(Vector2(1, 2),
                                                    Direction.BOTTOM),
                                               Door(Vector2(2, 1),
                                                    Direction.RIGHT),
                                            ])


class PassageH(Room):
    def __init__(self):
        super(PassageH, self).__init__(Vector2(4, 1),
                                       [Door(Vector2(0, 0),
                                             Direction.LEFT),
                                           Door(Vector2(3, 0),
                                                Direction.RIGHT),
                                        ])

    def can_be_adjacent(self, room):
        if isinstance(room, PassageH):
            return False
        return super(PassageH, self).can_be_adjacent(room)


class PassageV(Room):
    def __init__(self):
        super(PassageV, self).__init__(Vector2(1, 4),
                                       [Door(Vector2(0, 0),
                                             Direction.TOP),
                                           Door(Vector2(0, 3),
                                                Direction.BOTTOM),
                                        ])

    def can_be_adjacent(self, room):
        if isinstance(room, PassageV):
            return False
        return super(PassageV, self).can_be_adjacent(room)


placements = [
    RoomPlacement(PassageH, 2, 6),
    RoomPlacement(PassageV, 2, 6),
    RoomPlacement(MediumRoomTR, 2, 6),
    RoomPlacement(LargeRoomLBR, 2, 6),
    RoomPlacement(SmallRoomLB, 2, 6),
    RoomPlacement(Connector, 0, 6)
]
rp = RoomPlanner(SpawnRoom, placements)
rp.spawn()
