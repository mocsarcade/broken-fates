using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Include namespaces
using DungeonRooms; // Room class
using DungeonFloor; // Floor class
using DirectionClass; // Direction class

  public class RoomManager : MonoBehaviour {

  	//Singleton reference
  	public static RoomManager instance = null;

  	// Use this for initialization
  	void Awake () {
  		//Make RoomManager a Singleton
  		if (instance == null)
  			instance = this;
  		else if (instance != this)
  			Destroy(gameObject);
  	}

    //Define important constants
    public const int ROOM_CHANCE = 50;
    public static List<Room> mainRooms;

    public void createFloor(Floor thisFloor) {

      //Get the full list of rooms from floor class
      mainRooms = thisFloor.GetRooms();
      //Declare for layout of this floor
      Room[,] floorLayout = new Room[thisFloor.xSize,thisFloor.ySize];

      //Place each importantRoom in a random place on the floor
      foreach (Room _room in mainRooms) {
        bool repeat;
        do {
          repeat = false;
          _room.x = Random.Range(1, thisFloor.xSize-2);
          _room.y = Random.Range(1, thisFloor.xSize-2);
          repeat = CheckSpot(_room.x, _room.y, floorLayout);
          if(repeat == false) {
            floorLayout[_room.x, _room.y] = _room;
          }
        } while(repeat);

      }

      //Create Connections
        //Go through each reqired room that was placed and build a graph to connect them all to each other
        foreach(Room _room in mainRooms)
        {
          bool[] exits = FindDirections(_room.x, _room.y, floorLayout, _room.exits);
          //Go through each exit
          for(int i=0; i<4; i++) {
            if(exits[i]) {
              //Call recursive algorithm that begins making corridor rooms starting from each exit
              Tunnel(_room.x + DirectionUtility.getX(i), _room.y + DirectionUtility.getY(i), floorLayout, DirectionUtility.opposite(i), _room);
            }
            _room.exits[i] = exits[i];
          }
        }
    }

    //The Tunnel method will decide what kind of room is needed and call getCorridor to make rooms
    public static void Tunnel(int x, int y, Room[,] floorLayout, Direction from, Room _room) {
      //Decide directions this room will go to
      bool[] exits = FindDirections(x, y, floorLayout, _room);
      exits[DirectionUtility.getIndex(from)] = true;

      //Once all exits have been found, ask for a room to fill in this one
      if(x>=0 && y>=0 && x<floorLayout.GetLength(0) && y<floorLayout.GetLength(1))
        if(floorLayout[x,y] == null) {
          floorLayout[x,y] = GetCorridor(exits);
          //TODO: Instantiate Room






        }
      //Call Tunnel on each room this one can go to
      for(int i=0; i<4; i++) {
        if(exits[i]==true && i != DirectionUtility.getIndex(from)) {
          //if(CheckPoint(x+Direction.getX(i),y+Direction.getY(i), floorLayout)) {
            Tunnel(x + DirectionUtility.getX(i), y + DirectionUtility.getY(i), floorLayout, DirectionUtility.opposite(i), _room);
          //}
        }
      }
    }

    public static bool CheckSpot(int x, int y, Room[,] floorLayout) {
      int check = 0;
      if(floorLayout[x,y] == null) {
        check++;
      }
      for(int i=0; i<4; i++) {
        if(floorLayout[x+DirectionUtility.getX(i),y+DirectionUtility.getY(i)] == null) {
          check++;
        }
        for(int j=0; j<4; j++) {
          if(x+DirectionUtility.getX(i)+DirectionUtility.getX(j)>0 && x+DirectionUtility.getX(i)+DirectionUtility.getX(j)<floorLayout.GetLength(0) && y+DirectionUtility.getY(i)+DirectionUtility.getY(j)>0 && y+DirectionUtility.getY(i)+DirectionUtility.getY(j)<floorLayout.GetLength(1)) {
            if(floorLayout[x+DirectionUtility.getX(i)+DirectionUtility.getX(j),y+DirectionUtility.getY(i)+DirectionUtility.getY(j)] == null) {
              check++;
            }
          } else {
            check++;
          }
        }
      }
      if(check==21) {
        return false;
      }
      return true;
    }

    public static bool[] FindDirections(int x, int y, Room[,] floorLayout, Room _baseRoom) {
      bool[] exits = new bool[4]; bool noExit = true;
      List<int> goingDirections = new List<int> {0, 1, 2, 3};
      do {
        foreach(int i in goingDirections) {
          //Make sure we're inside the bounds of the loop
          int _x = x+DirectionUtility.getX(i);
          int _y = y+DirectionUtility.getY(i);
          if(_x<floorLayout.GetLength(0) && _x>=0 && _y<floorLayout.GetLength(1) && _y>=0) {
            if(floorLayout[_x,_y] == null) {
            //Randomize whether this exit will be used
              if(Random.Range(0,100) < ROOM_CHANCE) {
                  exits[i] = true;
                  noExit = false;
              }
            } else {
              //Check if this this room is a mainRoom
              foreach(Room _checkedRoom in mainRooms) {
                if(floorLayout[_x,_y]==_checkedRoom && _checkedRoom != _baseRoom) {
                  //If it is, check if this mainRoom is connected to another room yet
                  if((!_checkedRoom.isConnected() || !_baseRoom.isConnected()) && _checkedRoom.CheckExit(DirectionUtility.opposite(i))) {
                    //If it isn't, connect it
                    _baseRoom.Connect(_checkedRoom);
                    exits[i] = true;
                    noExit = false;
                  }
                }
              }
              //If we've made it through this far, there is no mainRoom we can go to so remove this room
              if(exits[i] == false) {
                goingDirections.Remove(i);
              }
            }
          } else {
            //Remove directions that are blocked off by other walls
            goingDirections.Remove(i);
          }
        }
      } while(noExit && goingDirections.Count>0);

      return exits;
    }

    public static bool[] FindDirections(int x, int y, Room[,] floorLayout, bool[] constraints) {
      bool[] exits = new bool[4]; bool noExit = true;
      List<int> goingDirections = new List<int> {0, 1, 2, 3};
      do {
        foreach(int i in goingDirections) {
          int _x = x+DirectionUtility.getX(i);
          int _y = y+DirectionUtility.getY(i);
          //Make sure we're inside the bounds of the loop and not overlapping with an already existing room
          if(_x<floorLayout.GetLength(0) && _x>=0 && _y<floorLayout.GetLength(1) && _y>=0) {
            if(floorLayout[_x,_y] == null) {
              //Randomize whether this exit will be used
              if(Random.Range(0,100) < ROOM_CHANCE) {
                  exits[i] = true;
                  noExit = false;
              }
            } else {
              //If the room seen is opening into this room, it's OK!
              if(floorLayout[_x,_y].CheckExit(DirectionUtility.opposite(i))) {
                exits[i] = true;
                noExit = false;
              } else {
                //Otherwise, yeah, remove it
                goingDirections.Remove(i);
              }
            }
          } else {
            //Remove directions that are blocked off by other walls
            goingDirections.Remove(i);
          }
        }
      } while(noExit && goingDirections.Count>0);

      return exits;
    }

    public static Room GetCorridor(bool[] exits) {
      //This room in the main code would get the corridor room required

      //TODO: WRITE THIS!!!
        //Find Room that fits specifications

        //External conditions: Don't do the same room too often

        //CREATE this room
      return null;


      //After writing this, don't forget to go back to Tunnel() to instantiate it
    }
}

//Namespace with Room class declared inside it
//using DungeonRooms;
/*
namespace DungeonManager {

	public class RoomManager : MonoBehaviour {

		protected GameObject[][] _floorRooms;

		//Room library for building rooms
		protected GameObject[][] roomLibrary;
		protected GameObject[][] usedRooms;

		private Dictionary<Direction, Direction> opposingEntrance = new Dictionary<Direction, Direction>();

		void Start() {
			opposingEntrance.Add(Direction.Left, Direction.Right);
			opposingEntrance.Add(Direction.Up, Direction.Down);
			opposingEntrance.Add(Direction.Right, Direction.Left);
			opposingEntrance.Add(Direction.Down, Direction.Up);
		}

		//This method is called whenever this world/castle is entered.
		//@param curFloor - All data about the current floor's "special" rooms, as well as treasure and floor size
			//ex: Floor of 9x5 rooms with a cutscene room Boss1 at 2x2 with the player starting at 8x3. The difficulty is 4
			//with the player's starting room being StartRoom1
		//The initialize builds the dungeon from randomly assorted rooms from the room resources folder for given difficulty
		//The dungeon starts at starting position for player and keeps building with proper entrances and exits matching
		//until full of dead ends, and then adds and replaces rooms to reach desired cutscene and treasure rooms
		//NOTE: All rooms must have a unique name and the
		public void BuildFloor(Floor curFloor) {
			//Build path graph using "Bridge" Algorithm

			//Go through each point on the graph and find a room that fits its qualifications
			//while() {
				//Find room
				//Create room
			//}
		}

		//Goes through every exit to build four lists of exits; one for each direction then take each
		//exit (max of three in one direction) and put it through opposingEntrance dictionary
		//Exits in the same direction can be found by comparing the exit variable to the direction variable (hidden integer variable)
		protected void BuildOffRoom(Room curRoom, bool firstCall) {
			//Declare runtime variables (requiredEntrances, deadEnds and direction array)
			int[] direction = {0,0};

			//Go through all four directions bordering curRoom
			foreach(Direction towardsDirection in Enum.GetValues(typeof(Direction))) {
				if(towardsDirection == Direction.Up || towardsDirection == Direction.Down) {
					direction[1] = (int) towardsDirection; //Basically {0, towardsDirection}, basically a vertical offset
				}
				if(towardsDirection == Direction.Up || towardsDirection == Direction.Down) {
					direction[0] = (int) towardsDirection; //Basically {towardsDirection, 0}, basically a horizontal offset
				}

				//if no room exists there, check if there are other bording rooms beside the soon-to-be-made room by for loop

					//Get exits of those rooms and put into the opposingEntrance dictionary
					//Add those rooms' required entrances for this room to the room's exit list

				//Check if room will be bordering any end of the floor

					//If so, add a direction to the deadEnds list

				//Call FindRoom with the exit lists for that direction and save in variable

				//Set transform position of Room object according to place in _floorRooms
						//(this will require either transform.position = ... or PrefabUtility....)

				//Add the room to _floorRooms

				//Call BuildOffRoom on the newly made room
				//BuildRoom(newRoom, false);
			}

			//Check firstCall. If true, we have made it all the way through the recursive tree

				//If that's the case, go through each occurence and use a djikstra algorithm
				//To see if a path leading between the occurence and the start exists

					//If not, start by using BuildRoom on occurence room.

					//Then call MoveTowards
		}

		protected void MoveTowards(Room curRoom, Room target) {
			//Use djikstra from curRoom towards target, keeping
			//track of the room where distance to target is lowest

			//If distance ever becomes 0, there is no need for adjustment

				//Take that room and make a list of its entrances and deadEnds

				//Add one entrance in direction of target (get rid of deadEnd if there as well)

				//Call FindRoom and replace current room with the newly found room

				//Do the same for the room it moved towards (a new entrance has been added, so
				//a similar entrance has to be placed on the adjacent room)
				//MoveInto(..., ..., target)

		}

		protected void MoveInto(int xPos, int yPos, Room target) {
				//Check if there is a room there at all

					//If there is...
					//Fill variable toRoom with room at xPos, yPos

					//Find toRoom's entrances and deadEnds

					//Add one entrance according to the entrance added by curRoom's MoveTowards call (get rid of deadEnd if there as well)

					//Call FindRoom and replace toRoom with newly found room

					//Call MoveTowards on toRoom (yes that is the same room as at xPos, yPos, which we just adjusted)


					//If there isn't...
					//Check on all directions for adjacent rooms and entrances

					//Put adjacent exits into opposingEntrance

					//Define direction variable to be filled in following if statements

						//If there is an adjacent room where target is CLOSER

							//add direction to that room

							//Call FindRoom and place newly found room in _floorRooms

						//If not

							//add direction to list in direction of target

							//Call FindRoom and place newly found room in _floorRooms

					//Call MoveInto on position beyond newly made room according to what's in direction

		}

		protected GameObject FindRoom(List<Exits> _exitsFromLastRoom, List<Exits> _deadEnds) {
			//Go through library of rooms

				//If this room fits the requirements

					//Save room in external varaible

					//Remove room from library of rooms

					//Add room to usedRooms library

					//Return room

			//If we have gone through the entire library and not found anything, go through usedRooms

				//If this room fits the requirements

					//Save room in external varaible

					//Remove room from library of rooms

					//Add room to usedRooms library

					//Return room


			//If there is no room, raise an exception and return null
			Debug.LogException(new Exception("No room with those specifications: " + _exitsFromLastRoom + " or dead ends " + _deadEnds), this);
			return null;
		}

		void OnTriggerEnter2D(Collider2D roomCollider) {
			//The camera has entered a new room, so build all surrounding rooms
			GameObject touchedRoom = roomCollider.gameObject;
			Vector2 roomPosition = new Vector2(
			touchedRoom.transform.position.x/GlobalRegistry.ROOM_WIDTH(),
			touchedRoom.transform.position.y/GlobalRegistry.ROOM_HEIGHT()
			);

			for(int i=-1; i<=1; i=i+2) {
				//Make sure this room to make is within x limits
				if(i+roomPosition.x >= 0 && i+roomPosition.x < _floorRooms.Length) {
					for(int j=-1; j<=1; j=j+2) {
						//Make sure this room to make is within y limits
						if(j+roomPosition.y >= 0 && j+roomPosition.y < _floorRooms[0].Length) {
							//Then build room
							Instantiate(_floorRooms[i + (int) roomPosition.x][j + (int) roomPosition.y]);
						}

					}
				}
			}

		}

	}

	public class Floor {
		public Vector2 floorSize;

		//Rooms with special purpose
		public GameObject[] occurences;
		public Vector2[] occurencePositions;

		public GameObject startingRoom;
		public Vector2 startingPosition;

		public int difficulty;

	}
}
*/
