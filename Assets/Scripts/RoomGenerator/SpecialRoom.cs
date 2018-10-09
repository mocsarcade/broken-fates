using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DirectionClass;
using DungeonRooms;

namespace SpecialDungeonRooms {

	public class SpecialRoom : Room {

		public GameObject upWall;
		public GameObject downWall;
		public GameObject leftWall;
		public GameObject rightWall;

		public void DisableRoom(Direction dir) {
			if(dir == Direction.UP) {
				upWall.SetActive(false);
			}
			if(dir == Direction.DOWN) {
				downWall.SetActive(false);
			}
			if(dir == Direction.LEFT) {
				leftWall.SetActive(false);
			}
			if(dir == Direction.RIGHT) {
				rightWall.SetActive(false);
			}
		}

		public void EnableRoom(Direction dir) {
			if(dir == Direction.UP) {
				upWall.SetActive(true);
			}
			if(dir == Direction.DOWN) {
				downWall.SetActive(true);
			}
			if(dir == Direction.LEFT) {
				leftWall.SetActive(true);
			}
			if(dir == Direction.RIGHT) {
				rightWall.SetActive(true);
			}
		}
	}

}
