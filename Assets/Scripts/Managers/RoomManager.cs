using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//Namespace with Exits enum declared inside it
using DungeonRooms;

namespace DungeonManager {

	public class RoomManager : MonoBehaviour {

		protected GameObject[][] _floorRooms;

		//Room library for building rooms
		protected GameObject[][] roomLibrary;
		protected GameObject[][] usedRooms;

		private Dictionary<Exits, Exits> opposingEntrance = new Dictionary<Exits, Exits>();

		void Start() {
			opposingEntrance.Add(Exits.Left, Exits.Right);
			opposingEntrance.Add(Exits.TopLeft, Exits.BottomLeft);
			opposingEntrance.Add(Exits.Top, Exits.Bottom);
			opposingEntrance.Add(Exits.TopRight, Exits.BottomRight);
			opposingEntrance.Add(Exits.Right, Exits.Left);
			opposingEntrance.Add(Exits.BottomRight, Exits.TopRight);
			opposingEntrance.Add(Exits.Bottom, Exits.Top);
			opposingEntrance.Add(Exits.BottomLeft, Exits.TopLeft);
		}

		//This method is called whenever this world/castle is entered.
		//@param curFloor - All data about the current floor's "special" rooms, as well as treasure and floor size
			//ex: Floor of 9x5 rooms with a cutscene room Boss1 at 2x2 with the player starting at 8x3. The difficulty is 4
			//with the player's starting room being StartRoom1
		//The initialize builds the dungeon from randomly assorted rooms from the room resources folder for given difficulty
		//The dungeon starts at starting position for player and keeps building with proper entrances and exits matching
		//until full of dead ends, and then adds and replaces rooms to reach desired cutscene and treasure rooms
		//NOTE: All rooms must have a unique name and the
		public void Initialize(Floor curFloor) {
			//Place First Room at proper position in _floorRooms

			//Place all occurences in floorRooms

			//Import roomLibrary using difficulty level

			//Call BuildOffRoom for first room
			//BuildRoom(..., true)
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

		public GameObject[] startingRoom;
		public Vector2 startingPosition;

		public int difficulty;

	}

	public enum Direction : int {
		Up = 1,
		Down = -1,
		Right = 1,
		Left = -1,
	}
}
