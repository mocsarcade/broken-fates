using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonRooms;

public class RoomTemplate : MonoBehaviour {
    public List<Room> topRooms;
    public List<Room> bottomRooms;
    public List<Room> leftRooms;
    public List<Room> rightRooms;

    public List<Room> getRooms(int direction) {
      switch(direction) {
        case 0:
        return topRooms;
        case 1:
        return bottomRooms;
        case 2:
        return leftRooms;
        case 3:
        return rightRooms;
        default:
        return new List<Room>();
      }
    }
}
