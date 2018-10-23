using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

using DirectionClass;
using DungeonRooms;

namespace SpecialDungeonRooms {

	public class SpecialRoom : Room {
		public bool initialized;

		public List<GameObject> upWall = new List<GameObject>();
		public List<GameObject> downWall = new List<GameObject>();
		public List<GameObject> leftWall = new List<GameObject>();
		public List<GameObject> rightWall = new List<GameObject>();

		public void Initialize() {
			if(!initialized && unPacked) {
				foreach (Renderer child in roomObjects){
					if (child.gameObject.name == "upWall"){
							upWall.Add(child.gameObject);
					}
					if (child.gameObject.name == "downWall"){
							downWall.Add(child.gameObject);
					}
					if (child.gameObject.name == "leftWall"){
							leftWall.Add(child.gameObject);
					}
					if (child.gameObject.name == "rightWall"){
							rightWall.Add(child.gameObject);
					}
					initialized = true;
				}
			}
		}

		public void DisableRoom(Direction dir) {
			if(dir == Direction.UP) {
				Timing.RunCoroutine(SetVisibility(upWall, false));
			}
			if(dir == Direction.DOWN) {
				Timing.RunCoroutine(SetVisibility(downWall,false));
			}
			if(dir == Direction.LEFT) {
				Timing.RunCoroutine(SetVisibility(leftWall,false));
			}
			if(dir == Direction.RIGHT) {
				Timing.RunCoroutine(SetVisibility(rightWall,false));
			}
		}

		public void EnableRoom(Direction dir) {
			if(dir == Direction.UP) {
				Timing.RunCoroutine(SetVisibility(upWall, true));
			}
			if(dir == Direction.DOWN) {
				Timing.RunCoroutine(SetVisibility(downWall,true));
			}
			if(dir == Direction.LEFT) {
				Timing.RunCoroutine(SetVisibility(leftWall,true));
			}
			if(dir == Direction.RIGHT) {
				Timing.RunCoroutine(SetVisibility(rightWall,true));
			}
		}

		private IEnumerator<float> SetVisibility(List<GameObject> list, bool flag) {
			//Check if roomList hasn't been loaded yet
			while(!initialized) {
				Initialize();
				yield return Timing.WaitForSeconds(0.25f);
			}
			//Disable wall
			foreach(GameObject wall in list) {
				wall.SetActive(flag);
			}
		}

		public override bool AddExit(Direction dir, Room[,] floorLayout) {
			Debug.Log("Removing wall at " + dir);
			if(exits[DirectionUtility.getIndex(dir)] == true) {
				DisableRoom(dir);
				return true;
			}
			return false;
		}
	}

}
