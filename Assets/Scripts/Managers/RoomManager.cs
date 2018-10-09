using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exception = System.Exception;
using System.Linq;

//Include namespaces
using DungeonRooms; // Room class
using DungeonFloor; // Floor class
using DirectionClass; // Direction class
using SpecialDungeonRooms;

  public class RoomManager : MonoBehaviour {

  	//Singleton reference
    public static RoomManager instance = null;
    public RoomTemplate roomList;

  	// Use this for initialization
  	void Awake () {
  		//Make RoomManager a Singleton
  		if (instance == null)
  			instance = this;
  		else if (instance != this)
  			Destroy(gameObject);
  	}

    //Define important constants
    public const int ROOM_CHANCE = 50;
    public static List<SpecialRoom> mainRooms;

    public void createFloor(Floor thisFloor) {
      roomList = GameObject.FindWithTag("Rooms").GetComponent<RoomTemplate>();

      //Get the full list of rooms from floor class
      mainRooms = thisFloor.GetRooms();
      mainRooms.Add(thisFloor.GetEntrance());
      //Declare for layout of this floor
      Room[,] floorLayout = new Room[thisFloor.xSize,thisFloor.ySize];

      //Place each importantRoom in a random place on the floor
      foreach (SpecialRoom _room in mainRooms) {
        bool repeat;
        do {
          repeat = false;
          _room.x = Random.Range(1, thisFloor.xSize-2);
          _room.y = Random.Range(1, thisFloor.xSize-2);
          repeat = CheckSpot(_room.x, _room.y, floorLayout);
          if(repeat == false) {
            //After going through exits, create this mainRoom
            floorLayout[_room.x, _room.y] = _room;
            Instantiate(_room.gameObject, new Vector3(_room.x*10, _room.y*-10, 0), Quaternion.identity);
          }
        } while(repeat);

      }

      //Create Connections
        //Go through each reqired room that was placed and build a graph to connect them all to each other
        foreach(SpecialRoom _room in mainRooms)
        {
          bool[] exits = FindDirections(_room.x, _room.y, floorLayout, _room.getExits());
          //Go through each exit
          for(int i=0; i<4; i++) {
            //_room.setExit(i, exits[i]);
            if(exits[i]) {
              //Call recursive algorithm that begins making corridor rooms starting from each exit
              Tunnel(_room.x + DirectionUtility.getX(i), _room.y + DirectionUtility.getY(i), floorLayout, DirectionUtility.opposite(i), _room);
              //Make this mainRoom open or close walls depending on which exits have been set to open
              _room.EnableRoom(DirectionUtility.getDirection(i));
            } else {
              _room.DisableRoom(DirectionUtility.getDirection(i));
            }

          }
        }
        //TODO: Find a way to know where the START is and place player there
        //Player.GetPlayer().SetPosition(new Vector2(), 0)
    }

    //The Tunnel method will decide what kind of room is needed and call getCorridor to make rooms
    public void Tunnel(int x, int y, Room[,] floorLayout, Direction from, Room _room) {
      //Decide directions this room will go to
      bool[] exits = FindDirections(x, y, floorLayout, _room);
      exits[DirectionUtility.getIndex(from)] = true;

      //Once all exits have been found, ask for a room to fill in this one
      if(x>=0 && y>=0 && x<floorLayout.GetLength(0) && y<floorLayout.GetLength(1))
        if(floorLayout[x,y] == null) {
          Room createdRoom = GetCorridor(exits);
          createdRoom.x = x;
          createdRoom.y = y;
          floorLayout[x,y] = createdRoom;
          Instantiate(createdRoom.gameObject, new Vector3(x*10, y*-10, 0), Quaternion.identity);

          //Call Tunnel on each room this one can go to
          for(int i=0; i<4; i++) {
            if(exits[i]==true && i != DirectionUtility.getIndex(from)) {
              if(floorLayout[x + DirectionUtility.getX(i),y + DirectionUtility.getY(i)] == null) {
                //if(CheckPoint(x+Direction.getX(i),y+Direction.getY(i), floorLayout)) {
                  Tunnel(x + DirectionUtility.getX(i), y + DirectionUtility.getY(i), floorLayout, DirectionUtility.opposite(i), _room);
                //}
              }
            }
          }
        }
    }

    public static bool CheckSpot(int x, int y, Room[,] floorLayout) {
      int check = 0;
      if(floorLayout[x,y] == null) {
        check++;
      }
      for(int i=0; i<4; i++) {
        if(floorLayout[x+DirectionUtility.getX(i),y+DirectionUtility.getY(i)] == null) {
          check++;
        }
        for(int j=0; j<4; j++) {
          if(x+DirectionUtility.getX(i)+DirectionUtility.getX(j)>0 && x+DirectionUtility.getX(i)+DirectionUtility.getX(j)<floorLayout.GetLength(0) && y+DirectionUtility.getY(i)+DirectionUtility.getY(j)>0 && y+DirectionUtility.getY(i)+DirectionUtility.getY(j)<floorLayout.GetLength(1)) {
            if(floorLayout[x+DirectionUtility.getX(i)+DirectionUtility.getX(j),y+DirectionUtility.getY(i)+DirectionUtility.getY(j)] == null) {
              check++;
            }
          } else {
            check++;
          }
        }
      }
      if(check==21) {
        return false;
      }
      return true;
    }

    public static bool[] FindDirections(int x, int y, Room[,] floorLayout, Room _baseRoom) {
      bool[] exits = new bool[4]; bool noExit = true;
      List<int> goingDirections = new List<int> {0, 1, 2, 3};
      do {
        foreach(int i in goingDirections) {
          //Make sure we're inside the bounds of the loop
          int _x = x+DirectionUtility.getX(i);
          int _y = y+DirectionUtility.getY(i);
          if(_x<floorLayout.GetLength(0) && _x>=0 && _y<floorLayout.GetLength(1) && _y>=0) {
            if(floorLayout[_x,_y] == null) {
            //Randomize whether this exit will be used
              if(Random.Range(0,100) < ROOM_CHANCE) {
                  exits[i] = true;
                  noExit = false;
              }
            } else {
              //Check if room this one is facing is already open to this room
              bool[] roomExits = floorLayout[_x,_y].getExits();
              //Otherwise, Check if this this room is a mainRoom
              foreach(SpecialRoom _checkedRoom in mainRooms) {
                if(floorLayout[_x,_y]==_checkedRoom && ((Room) _checkedRoom) != _baseRoom) {
                  //If it is, check if this mainRoom is connected to another room yet
                  if((!_checkedRoom.isConnected() || !_baseRoom.isConnected()) && _checkedRoom.CheckExit(DirectionUtility.opposite(i))) {
                    //If it isn't, connect it
                    _baseRoom.Connect(_checkedRoom);
                    exits[i] = true;
                    noExit = false;
                    //Open this entrance of the mainRoom
                    _checkedRoom.EnableRoom(DirectionUtility.opposite(i));
                  }
                }
              }
              //If we haven't found a mainRoom, check again to see if it's just a room already open on this direction
              if(exits[i] == false && roomExits[DirectionUtility.getIndex(DirectionUtility.opposite(i))] == true) {
                exits[i] = true;
                noExit = false;
              }
              //If we've made it through this far, there is no hope of connecting here so remove this direction
              if(exits[i] == false) {
                goingDirections.Remove(i);
                break;
              }
            }
          } else {
            //Remove directions that are blocked off by other walls
            goingDirections.Remove(i);
            break;
          }
        }
      } while(noExit && goingDirections.Count>0);

      return exits;
    }

    public static bool[] FindDirections(int x, int y, Room[,] floorLayout, bool[] constraints) {
      bool[] exits = new bool[4]; bool noExit = true;
      List<int> goingDirections = new List<int> {0, 1, 2, 3};
      for(int i=0; i<4; i++) {
        if(constraints[i] == false) {
          goingDirections.Remove(i);
        }
      }
      do {
        foreach(int i in goingDirections) {
          int _x = x+DirectionUtility.getX(i);
          int _y = y+DirectionUtility.getY(i);
          //Make sure we're inside the bounds of the loop and not overlapping with an already existing room
          if(_x<floorLayout.GetLength(0) && _x>=0 && _y<floorLayout.GetLength(1) && _y>=0) {
            if(floorLayout[_x,_y] == null) {
              //Randomize whether this exit will be used
              if(Random.Range(0,100) < ROOM_CHANCE) {
                  exits[i] = true;
                  noExit = false;
              }
            } else {
              //If the room seen is opening into this room, it's OK!
              if(floorLayout[_x,_y].CheckExit(DirectionUtility.opposite(i))) {
                exits[i] = true;
                noExit = false;
              } else {
                //Otherwise, yeah, remove it
                goingDirections.Remove(i);
                break;
              }
            }
          } else {
            //Remove directions that are blocked off by other walls
            goingDirections.Remove(i);
            break;
          }
        }
      } while(noExit && goingDirections.Count>0);

      return exits;
    }

    private Room GetCorridor(bool[] exits) {
        //Find Room that fits specifications
        List<Room> rooms = new List<Room>();
        //Find first direction and populate list
        bool repeat = true; int i = 0;
        while(repeat) {
          if(exits[i]) {
            rooms = roomList.getRooms(i);
            repeat = false;
          }
          i++;
          if(i>=4) {
      			Debug.LogException(new Exception("Looking for a room with no entrances!"), this);
          }
        }

        //Now go through all directions again and check for intersection
        for(int j=0; j<4; j++) {
          //If their exit is true, intersect
          if(exits[j]) {
            rooms = rooms.Intersect(roomList.getRooms(j)).ToList();
          }
          //If exit is false, subtract
          if(!exits[j]) {
            rooms = rooms.Except(roomList.getRooms(j)).ToList();
          }
        }

        //Once finding a full list of available rooms, choose a random one and return it
        //if(rooms.Count > 0) {
          return rooms[Random.Range(0, rooms.Count-1)];
        //}
        //TODO: External conditions: Don't do the same room too often

    }
}
