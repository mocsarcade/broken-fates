using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

using DirectionClass; // Direction class

namespace DungeonRooms {

	public class Room : MonoBehaviour {

		public int order;
		public void setOrder(int _order) {
			order = _order;
		}

		protected bool unPacked;
		//Logistic info
	  public bool connected;
	  public int x;
	  public int y;
	  public bool[] exits;
		protected List<Renderer> roomObjects = new List<Renderer>();
		public Room baseRoom;

		//Editor-changed-values
		public bool exitUp; public bool exitDown; public bool exitLeft; public bool exitRight;

		//Is called when the editor updates any of the public variables (exitUp,Down,etc)
		void OnValidate()
		{
			//Update exits whenever the four bools are updated
			exits = new bool[4];
			exits[0] = exitUp;
			exits[1] = exitDown;
			exits[2] = exitLeft;
			exits[3] = exitRight;
		}

		//RoomTemplate object (THIS IS NO LONGER USED)
		//private RoomTemplate template;

    // Use this for initialization
    /*void Start()
    {
        template = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplate>();
        template.rooms.Add(this.gameObject);
    }*/

		// Use this for initialization
		public virtual void Awake () {
			//CreateRoom(0,0);
			if(!unPacked)
				UnpackRoom();
		}

		/*
		// Update is called once per frame
		private void SaveScene () {
			object[] obj = GameObject.FindObjectsOfType(typeof (GameObject));
	    foreach (object o in obj)
	    {
	        GameObject g = (GameObject) o;
					if(g.tag == "Untagged") {
						roomObjects.Add(Instantiate(g));
					}
	    }
		}
		// Update is called once per frame
		private void CreateRoom (int xUnit, int yUnit) {
			foreach(GameObject toCreate in roomObjects) {
				Instantiate(toCreate, toCreate.transform.position + new Vector3(xUnit * ROOM_WIDTH, yUnit * ROOM_HEIGHT), Quaternion.identity);
			}
		}*/

		private void UnpackRoom() {
			Renderer childRenderer;
			while(transform.childCount > 0) {
				childRenderer = transform.GetChild(0).gameObject.GetComponent<Renderer>();
				if(childRenderer) {
					roomObjects.Add(childRenderer);
				}
				Unpack(transform.GetChild(0), 5);
			}
			unPacked = true;
		}

		//Sets the object, if it is a child of the room to be a child of null
		//If it is a child of something that is not the room, go up the hierarchy and try again
		//This recursive method goes a max of stackMax times before giving up unPacking the child
		private void Unpack(Transform child, int stackMax) {
			if(child.parent == transform || child.parent == null) {
				child.parent = null;
			} else if(stackMax > 0) {
				Unpack(child.parent, stackMax-1);
			}
		}

		public virtual IEnumerator<float> DestroyRoom() {
			foreach(Renderer roomPiece in roomObjects) {
				if(roomPiece != null) {
					if(roomPiece.isVisible == false) {
						Destroy(roomPiece.gameObject);
					}
				}
			}
			yield return Timing.WaitForSeconds(1f);
		}

	  public bool CheckExit(Direction checkMe) {
			exits = validateExits();
	    return exits[DirectionUtility.getIndex(checkMe)];
	  }

	  protected virtual bool TestExit(int direction) {
			return exits[direction];
	  }

		public bool[] getExits() {
			exits = validateExits();
	    return exits;
	  }

		public virtual bool AddExit(Direction dir, Room[,] floorLayout) {
			if(exits[DirectionUtility.getIndex(dir)] == false) {
				exits[DirectionUtility.getIndex(dir)] = true;
				RoomManager.instance.ReplaceRoom(this, exits, floorLayout);
			}
			Debug.Log("Replacing Room " + x + " and " + y);
			return true;
		}

		private bool[] validateExits() {
			exits = new bool[4];
			exits[0] = exitUp;
			exits[1] = exitDown;
			exits[2] = exitLeft;
			exits[3] = exitRight;
			return exits;
		}

		/*public void setExit(int index, bool toSet) {
	    exits[index] = toSet;
	  }*/

		public void setBase(Room _base) {
			baseRoom = _base;
		}

	  public bool isConnected() {
			if(baseRoom != null) {
				return baseRoom.isConnected();
			} else {
				return connected;
			}
	  }

		public virtual void BuildTowards(Room target, Room _source, Room[,] floorLayout) {
      List<int> goingDirections = new List<int> {0, 1, 2, 3};
      foreach(int i in goingDirections) {
				int _x = x+DirectionUtility.getX(i);
				int _y = y+DirectionUtility.getY(i);
				int dist = Mathf.Abs(x-target.x) + Mathf.Abs(y-target.y);
				int directDist = Mathf.Abs(_x-target.x) + Mathf.Abs(_y-target.y);

				if(directDist < dist && _x<floorLayout.GetLength(0) && _x>=0 && _y<floorLayout.GetLength(1) && _y>=0) {
					//Build Towards that direction
					if(floorLayout[_x,_y] == null) {
						Debug.Log("Tunneling " + x + ", " + y + " into " + _x + ", " + _y);
						//If there is no room here, Tunnel!
						RoomManager.instance.Tunnel(_x, _y, floorLayout, DirectionUtility.opposite(i), _source);
						bool status = AddExit(DirectionUtility.getDirection(i), floorLayout);
						if(status == true) {
							return;
						}
					} else if(TestExit(i) == true) {
						Debug.Log("Building towards " + _x + ", " + _y);
						floorLayout[_x,_y].BuildTowards(target, _source, floorLayout);
						return;
					} else {
						//If there is a room there, connect them!
						if(floorLayout[_x,_y].getBaseRoom() != _source)
							floorLayout[_x,_y].Connect(_source);
						RoomManager.instance.FuseRooms(this, floorLayout[_x,_y], floorLayout);
						return;
					}
					//End
					return;
				}
			}
			//If we've made it here, no direction is closer to the target, which means THIS IS THE TARGET!
			_source.Connect(target);
		}

		public virtual Room getBaseRoom() {
			return baseRoom;
		}

	  public void Connect(Room _connectedRoom) {
			if(baseRoom != null) {
				baseRoom.Connected();
			} else {
				Debug.Log(gameObject.name + " is connected!");
				connected=true;
			}
	    _connectedRoom.Connected();
	  }

	  public void Connected() {
			if(baseRoom != null) {
				baseRoom.Connected();
			}
			Debug.Log(gameObject.name + " is connected!");
	    connected=true;
	  }

	}
}
