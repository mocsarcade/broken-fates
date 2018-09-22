/*
 * Class that represents a room. Will be used as a script on each room prefab in the main program
 *
 * @author Robert Ziang, Sara Harvey, Tyler Harris, Connor McPherson
 */
import java.util.*;

public class Room {

  //Since dungeon is portrayed in boxes, doesn't that mean all rooms have same
  //size of one unit on the floor's grid?
  //public int xSize;
  //public int ySize;
  public int x;
  public int y;
  public boolean[] exits;

  public Room(boolean[] _exits) {
  //public Room(int _xSize, int _ySize, List<FloorManager.Direction> _exits) {
    //xSize = _xSize;
    //ySize = _ySize;
    exits = _exits;
  }

  public Room() {
    exits = new boolean[4];
    exits[FloorManager.Direction.DOWN.getIndex()] = true;
    exits[FloorManager.Direction.LEFT.getIndex()] = true;
    exits[FloorManager.Direction.RIGHT.getIndex()] = true;
    exits[FloorManager.Direction.UP.getIndex()] = true;
  }

  public boolean CheckExit(FloorManager.Direction checkMe) {
    return exits[checkMe.getIndex()];
  }

}
