using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpecialDungeonRooms; // Room class

namespace DungeonFloor {
  public class Floor {

    public int xSize;
    public int ySize;
    private List<SpecialRoom> room;
    private SpecialRoom Entrance;

    public Floor(int _xSize, int _ySize, List<SpecialRoom> _room, SpecialRoom _entrance) {
      xSize = _xSize;
      ySize = _ySize;
      room = _room;
      Entrance = _entrance;
    }

    public List<SpecialRoom> GetRooms() {
      return room;
    }

    public SpecialRoom GetEntrance() {
      return Entrance;
    }

  }
}
