/*
 * Prototype for Dungeon Building Algorithm. Places required rooms and then puts passageways
 * Between them
 */

/**
 *
 * @author Robert Ziang, Sara Harvey, Tyler Harris, Connor McPherson
 */
import java.util.*;

public class FloorManager {
    //Define important constants
    public static final int ROOM_CHANCE = 50;

    public static void main(String[] args) {

      //Define this floor's size and required rooms
      //These main rooms include the start, exit and secret. These varaibles will be held
      //in a room object's script, although the internal data of what is IN the room is not necessarily known
      List<Room> importantRooms = new ArrayList<>();//<Room>();
      importantRooms.add(new Room());
      importantRooms.add(new Room());
      importantRooms.add(new Room());
      //This is the floor we're making this section would be the input of the
      //generation function in the Unity version
      Floor thisFloor = new Floor(10,5,importantRooms);


      //Get the full list of rooms from floor class
      //This is something more of a mock example since we already have the rooms in mainRooms XD
      List<Room> mainRooms = thisFloor.GetRooms();
      //Declare for layout of this floor
      Room[][] floorLayout = new Room[thisFloor.xSize][thisFloor.ySize];


      Random rand = new Random();
      //Place each importantRoom in a random place on the floor
      for (Room _room : mainRooms) {
        boolean repeat;
        do {
          repeat = false;
          _room.x = rand.nextInt(floorLayout.length);
          _room.y = rand.nextInt(floorLayout[0].length);
          if(floorLayout[_room.x][_room.y] == null) {
            floorLayout[_room.x][_room.y] = _room;
          } else {
            //If this room is on another room, randomize again to a new room
            repeat = true;
          }
        } while(repeat);

      }

      //Create Connections
        //Go through each reqired room that was placed and build a graph to connect them all to each other
        for(Room _room : mainRooms)
        {
          boolean[] exits = FindDirections(_room.x, _room.y, floorLayout, _room.exits);
          //Go through each exit
          for(int i=0; i<4; i++) {
            if(exits[i]) {
              //Call recursive algorithm that begins making corridor rooms starting from each exit
              Tunnel(_room.x + Direction.getX(i), _room.y + Direction.getY(i), floorLayout, Direction.opposite(i));
            }
            _room.exits[i] = exits[i];
          }
        }






        /*
        *  Connor should never let me touch his computer
        *    Yeh.
        */

      //Print out graph of finished dungeon floor
      for(int col=0; col < floorLayout[0].length; col++) {
        for(int roomPrint=0; roomPrint < 3; roomPrint++) {
          for(int row=0; row < floorLayout.length; row++) {
            if(floorLayout[row][col] != null) {
              //Printing a room
              switch(roomPrint) {
                case 0:
                  System.out.print("O");
                  if(floorLayout[row][col].CheckExit(Direction.UP)) {
                    System.out.print(" ");
                  } else {
                    System.out.print("O");
                  }
                  System.out.print("O");
                break;
                case 1:
                  if(floorLayout[row][col].CheckExit(Direction.LEFT)) {
                    System.out.print(" ");
                  } else {
                    System.out.print("O");
                  }
                  System.out.print(" ");
                  if(floorLayout[row][col].CheckExit(Direction.RIGHT)) {
                    System.out.print(" ");
                  } else {
                    System.out.print("O");
                  }
                break;
                case 2:
                  System.out.print("O");
                  if(floorLayout[row][col].CheckExit(Direction.DOWN)) {
                    System.out.print(" ");
                  } else {
                    System.out.print("O");
                  }
                  System.out.print("O");
                break;
              }
            } else {
              System.out.print("-");
              System.out.print("-");
              System.out.print("-");
            }
          }
          System.out.println();
        }
      }

    }

    //The Tunnel method will decide what kind of room is needed and call getCorridor to make rooms
    public static void Tunnel(int x, int y, Room[][] floorLayout, Direction from) {
      //Decide directions this room will go to
      boolean[] exits = FindDirections(x, y, floorLayout);
      exits[from.getIndex()] = true;

      //Once all exits have been found, ask for a room to fill in this one
      if(x>=0 && y>=0 && x<floorLayout.length && y<floorLayout[0].length)
        if(floorLayout[x][y] == null)
          floorLayout[x][y] = GetCorridor(exits);
      //Call Tunnel on each room this one can go to
      for(int i=0; i<4; i++) {
        if(exits[i]==true && i != from.getIndex()) {
          //if(CheckPoint(x+Direction.getX(i),y+Direction.getY(i), floorLayout)) {
            System.out.println(x + " and " + y + " going in direction " + i + " going to " + (x + Direction.getX(i)) + "," + (y + Direction.getY(i)) + " which is " + floorLayout[x + Direction.getX(i)][y + Direction.getY(i)]);
            Tunnel(x + Direction.getX(i), y + Direction.getY(i), floorLayout, Direction.opposite(i));
          //}
        }
      }
    }

    public static boolean[] FindDirections(int x, int y, Room[][] floorLayout) {
      boolean[] exits = new boolean[4]; boolean noExit = true;
      Random rand = new Random();
      List<Integer> goingDirections = new ArrayList<>(Arrays.asList(0, 1, 2, 3));
      do {
        int i=0;
        Iterator<Integer> iter = goingDirections.iterator();
        while(iter.hasNext()) {
          i = iter.next();
          //Make sure we're inside the bounds of the loop
          if(CheckPoint(x+Direction.getX(i),y+Direction.getY(i), floorLayout)) {
            //Randomize whether this exit will be used
            if(rand.nextInt(100) < ROOM_CHANCE) {
                exits[i] = true;
                noExit = false;
            }
          } else {
            //Remove directions that are blocked off by other walls
            iter.remove();
          }
        }
      } while(noExit && goingDirections.size()>0);

      return exits;
    }

    public static boolean[] FindDirections(int x, int y, Room[][] floorLayout, boolean[] constraints) {
      boolean[] exits = new boolean[4]; boolean noExit = true;
      Random rand = new Random();
      List<Integer> goingDirections = new ArrayList<>(Arrays.asList(0, 1, 2, 3));
      do {
        int i=0;
        Iterator<Integer> iter = goingDirections.iterator();
        while(iter.hasNext()) {
          i = iter.next();
          //Make sure we're inside the bounds of the loop
          if(CheckPoint(x+Direction.getX(i),y+Direction.getY(i), floorLayout) && constraints[i]) {
            //Randomize whether this exit will be used
            if(rand.nextInt(100) < ROOM_CHANCE) {
                exits[i] = true;
                noExit = false;
            }
          } else {
            //Remove directions that are blocked off by other walls
            iter.remove();
          }
        }
      } while(noExit && goingDirections.size()>0);

      return exits;
    }

    public static boolean CheckPoint(int x, int y, Room[][] floorLayout) {
      if(x<floorLayout.length && x>=0 && y<floorLayout[0].length && y>=0) {
        if(floorLayout[x][y] == null) {
          return true;
        }
      }
      return false;
    }

    public static Room GetCorridor(boolean[] exits) {
      //This room in the main code would get the corridor room required
      //This version just makes a new room with the right stipulations
      return new Room(exits);
    }

    public enum Direction {

      UP(0),DOWN(1),LEFT(2),RIGHT(3);

      // declaring private variable for getting values
      private int arrayValue;

      // enum constructor - cannot be public or protected
      private Direction(int arrayValue)
      {
          this.arrayValue = arrayValue;
      }

      // getter method
      public int getIndex()
      {
          return this.arrayValue;
      }

      public static Direction opposite(Direction in) {
        if(in == Direction.UP) {
          return Direction.DOWN;
        }
        if(in == Direction.DOWN) {
          return Direction.UP;
        }
        if(in == Direction.LEFT) {
          return Direction.RIGHT;
        }
        if(in == Direction.RIGHT) {
          return Direction.LEFT;
        }
        return null;
      }
      public static Direction opposite(int in) {
        if(in == 0) {
          return Direction.DOWN;
        }
        if(in == 1) {
          return Direction.UP;
        }
        if(in == 2) {
          return Direction.RIGHT;
        }
        if(in == 3) {
          return Direction.LEFT;
        }
        return null;
      }

      public int getX() {
        if(arrayValue == 2 || arrayValue == 3) {
          return (arrayValue % 2) * 2 - 1;
        } else {
          return 0;
        }
      }

      public int getY() {
        if(arrayValue == 1 || arrayValue == 0) {
          return arrayValue * 2 - 1;
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
