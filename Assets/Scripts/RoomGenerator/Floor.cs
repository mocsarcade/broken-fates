﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DungeonRooms; // Room class

namespace DungeonFloor {
  public class Floor {

    public int xSize;
    public int ySize;
    private List<Room> room;

    public Floor(int _xSize, int _ySize, List<Room> _room) {
      xSize = _xSize;
      ySize = _ySize;
      room = _room;
    }

    public List<Room> GetRooms() {
      return room;
    }

  }
}
