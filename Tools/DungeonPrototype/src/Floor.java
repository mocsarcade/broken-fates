/*
 * Class that represents a single floor and tracks its boundaries
 */

/**
 *
 * @author Robert Ziang, Sara Harvey, Tyler Harris, Connor McPherson
 */
import java.util.*;

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
