using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplate : MonoBehaviour {
    public List<GameObject> topRooms;
    public List<GameObject> bottomRooms;
    public List<GameObject> leftRooms;
    public List<GameObject> rightRooms;

    public List<GameObject> getRooms(int direction) {
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
        return new List<GameObject>();
      }
    }
}
