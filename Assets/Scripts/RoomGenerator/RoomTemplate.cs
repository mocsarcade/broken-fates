using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonRooms;

public class RoomTemplate : MonoBehaviour {
    public List<Room> topRooms;
    public List<Room> bottomRooms;
    public List<Room> leftRooms;
    public List<Room> rightRooms;

    void Awake() {
      //TODO: Somehow get this Floor's level

      //Import all rooms from this floor's level file (Resources FloorX, x=floor number)

      //Sort imported rooms into the four Lists. The same room CAN be in all four lists
      //If it has an exit in all four directions

    }

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
