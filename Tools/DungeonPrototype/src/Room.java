/*
 * Class that represents a room. Will be used as a script on each room prefab in the main program
 *
 * @author Robert Ziang, Sara Harvey, Tyler Harris, Connor McPherson
 */
import java.util.*;

public class Room {
  //This is just a variable for printing disregard it in making the # version:
  public boolean isMain;

  //Since dungeon is portrayed in boxes, doesn't that mean all rooms have same
  //size of one unit on the floor's grid?
  //public int xSize;
  //public int ySize;
  private boolean connected;
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
    isMain=true;
  }

  public boolean CheckExit(FloorManager.Direction checkMe) {
    return exits[checkMe.getIndex()];
  }

  public boolean isConnected() {
    return connected;
  }

  public void Connect(Room _connectedRoom) {
    connected=true;
    _connectedRoom.Connected();
  }

  public void Connected() {
    connected=true;
  }

}
