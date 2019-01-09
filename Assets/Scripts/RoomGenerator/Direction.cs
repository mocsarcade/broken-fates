using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DirectionClass {

	public enum Direction {
		UP = 0,
		DOWN = 1,
		LEFT = 2,
		RIGHT = 3
	}

	public class DirectionUtility {

			public const Direction UP = Direction.UP;
			public const Direction DOWN = Direction.DOWN;
			public const Direction LEFT = Direction.LEFT;
			public const Direction RIGHT = Direction.RIGHT;

      // getter method
      public static int getIndex(Direction dir)
      {
          return (int) dir;
      }

			// getter method
      public static Direction getDirection(int dir)
      {
				if (dir == 0) {
          return Direction.UP;
        }
				else if(dir == 1) {
          return Direction.DOWN;
        }
				else if(dir == 2) {
          return Direction.LEFT;
        }
        else { //In this case dir == Direciton.RIGHT
          return Direction.RIGHT;
        }
      }

      public static Direction opposite(Direction dir) {
        if(dir == Direction.UP) {
          return Direction.DOWN;
        }
				else if (dir == Direction.DOWN) {
          return Direction.UP;
        }
				else if(dir == Direction.LEFT) {
          return Direction.RIGHT;
        }
        else { //In this case dir == Direciton.RIGHT
          return Direction.LEFT;
        }
      }

      public static Direction opposite(int dir) {
        if(dir == 0) {
          return Direction.DOWN;
        }
        else if(dir == 1) {
          return Direction.UP;
        }
        else if(dir == 2) {
          return Direction.RIGHT;
        }
        else { //In this case dir == 3
          return Direction.LEFT;
        }
      }

      public static int getX(Direction dir) {
        if(getIndex(dir) == 2 || getIndex(dir) == 3) {
          return (getIndex(dir) % 2) * 2 - 1;
        } else {
          return 0;
        }
      }

      public static int getY(Direction dir) {
        if(getIndex(dir) == 1 || getIndex(dir) == 0) {
          return (getIndex(dir) * 2 - 1);
        } else {
          return 0;
        }
      }

      public static int getX(int arrayValue) {
        if(arrayValue >= 2 && arrayValue <= 3) {
          return (arrayValue % 2) * 2 - 1;
        } else {
          return 0;
        }
      }

      public static int getY(int arrayValue) {
        if(arrayValue == 1 || arrayValue == 0) {
          return arrayValue * 2 - 1;
        } else {
          return 0;
        }
      }

  }

}
