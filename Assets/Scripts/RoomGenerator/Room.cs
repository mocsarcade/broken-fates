using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

using DirectionClass; // Direction class

namespace DungeonRooms {

	public class Room : MonoBehaviour {

		//Logistic info
	  private bool connected;
	  public int x;
	  public int y;
	  public bool[] exits;
		private List<Renderer> roomObjects = new List<Renderer>();

		//RoomTemplate object (THIS IS NO LONGER USED)
		//private RoomTemplate template;

    // Use this for initialization
    /*void Start()
    {
        template = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplate>();
        template.rooms.Add(this.gameObject);
    }*/

		// Use this for initialization
		void Awake () {
			//CreateRoom(0,0);
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
				if(roomPiece.isVisible == false) {
					roomObjects.Remove(roomPiece);
					Destroy(roomPiece.gameObject);
				}
			}
			yield return Timing.WaitForSeconds(1f);
		}

	  public bool CheckExit(Direction checkMe) {
	    return exits[DirectionUtility.getIndex(checkMe)];
	  }

	  public bool isConnected() {
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
}
