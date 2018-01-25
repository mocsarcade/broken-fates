using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//IMPORTANT: When adding this script as a component to the camera, you must set "floorTemplate" to the Prefab "Template Floor" manually in the inspector

public class FloorGenerator : MonoBehaviour {

	public static int width;
	public static int height;
	public GameObject floorTemplate;
	public Vector3 posit;
	public Vector2 playerPosit;
	public int[] LRBound;
	public int[] UDBound;

	// Create floor
	void Start () {
		//Initialize camera's position variables. THIS SCRIPT USES CAMERA'S POSITION, NOT THE PLAYER'S
		playerPosit = gameObject.transform.position;
		//Initialize screen size variables
		width = Screen.width;
		height = Screen.width;
		//Initialize bound ints. These will keep track of the x,y coordinates of the uppermost, leftmost, rightmost, and lowermost floor tiles
		LRBound = new int[2]; LRBound[0]=(-width/40-1); LRBound[1]=(width/40 + 1);
		UDBound = new int[2]; UDBound[0]=(-height/40-1); UDBound[1]=(height/40 + 1);
		//These for loops allow creation of floorspaces for every tile onscreen
		for (int x = LRBound[0]; x <= LRBound[1]; x += 1) {
				for (int y = UDBound[0]; y <= UDBound[1]; y += 1) {
				//Set up position of new cloned
				posit.x = x; posit.y = y; posit.z = 0;
				//Make cloned object
				//Variable tempClone is used in naming instantiated objects
				var tempClone = Instantiate(floorTemplate, posit, Quaternion.identity);
				//Name new clone a unique name so it can be referenced when floor must be destroyed
				tempClone.gameObject.name = "ClonedFloor: " + x + " " + y;
			}
		}
	}
	
	// Checks that player has moved, and if he has, add/destroy floor spaces
	void Update () {
		playerPosit = gameObject.transform.position;
		//Four if statements check the players' position compared against the bounds. If the player is further than normal, move the floor.
		if(playerPosit.x + width/40 > LRBound[1]) {
 			//Add new tiles on the right
				//Change right bound as new blocks have been made
				LRBound[1]++;
				//Each brick is made by its x and y coordinate. posit is the vector with these coordinates
				//This command initializes x, which will stay the same throughout the entire for loop
				posit.x = LRBound[1];
				for (int y = UDBound[0]; y <= UDBound[1]; y += 1) {
					//Set up y and z of new cloned, as the y position changes with each partition of the for loop
					posit.y = y; posit.z = 0;
					//Make cloned object
					//Variable tempClone is used to store object reference to name instantiated objects
					var tempClone = Instantiate(floorTemplate, posit, Quaternion.identity);
					//Name new clone a unique name so it can be referenced when floor must be destroyed
					tempClone.gameObject.name = "ClonedFloor: " + posit.x + " " + posit.y;
				}
			//Destroy old tiles on the opposite side
				for (int y = UDBound[0]; y <=UDBound[1]; y += 1) {
					//Find and destroy floor tile according to its name
					Destroy(GameObject.Find("ClonedFloor: " + LRBound[0] + " " + y));
				}
			//Change left bound as new blocks have been made
			LRBound[0]++;
		} else if(playerPosit.x - width/40 < LRBound[0]) {
			//Add new tiles on the left
				//Change right bound as new blocks have been made
				LRBound[0]--;
				//Each brick is made by its x and y coordinate. posit is the vector with these coordinates
				//This command initializes x, which will stay the same throughout the entire for loop
				posit.x = LRBound[0];
				for (int y = UDBound[0]; y <= UDBound[1]; y += 1) {
					//Set up y and z of new cloned, as the y position changes with each partition of the for loop
					posit.y = y; posit.z = 0;
					//Make cloned object
					//Variable tempClone is used to store object reference to name instantiated objects
					var tempClone = Instantiate(floorTemplate, posit, Quaternion.identity);
					//Name new clone a unique name so it can be referenced when floor must be destroyed
					tempClone.gameObject.name = "ClonedFloor: " + posit.x + " " + posit.y;
				}
			//Destroy old tiles on the opposite side
				for (int y = UDBound[0]; y <=UDBound[1]; y += 1) {
					//Find and destroy floor tile according to its name
					Destroy(GameObject.Find("ClonedFloor: " + LRBound[1] + " " + y));
				}
			//Change right bound as new blocks have been made
			LRBound[1]--;
		}
		
		if(playerPosit.y + height/40 > UDBound[1]) {
 			//Add new tiles on the right
				//Change upper bound as new blocks have been made
				UDBound[1]++;
				//Each brick is made by its x and y coordinate. posit is the vector with these coordinates
				//This command initializes y, which will stay the same throughout the entire for loop
				posit.y = UDBound[1];
				for (int x = LRBound[0]; x <= LRBound[1]; x += 1) {
					//Set up x and z of new cloned, as the y position changes with each partition of the for loop
					posit.x = x; posit.z = 0;
					//Make cloned object
					//Variable tempClone is used to store object reference to name instantiated objects
					var tempClone = Instantiate(floorTemplate, posit, Quaternion.identity);
					//Name new clone a unique name so it can be referenced when floor must be destroyed
					tempClone.gameObject.name = "ClonedFloor: " + posit.x + " " + posit.y;
				}
			//Destroy old tiles on the opposite side
				for (int x = LRBound[0]; x <= LRBound[1]; x += 1) {
					//Find and destroy floor tile according to its name
					Destroy(GameObject.Find("ClonedFloor: " + x + " " + UDBound[0]));
				}
			//Change lower bound as new blocks have been made
			UDBound[0]++;
		} else if(playerPosit.y - height/40 < UDBound[0]) {
			//Add new tiles on the bottom
				//Change lower bound as new blocks are being made
				UDBound[0]--;
				//Each brick is made by its x and y coordinate. posit is the vector with these coordinates
				//This command initializes y, which will stay the same throughout the entire for loop
				posit.y = UDBound[0];
				for (int x = LRBound[0]; x <= LRBound[1]; x += 1) {
					//Set up x and z of new cloned, as the y position changes with each partition of the for loop
					posit.x = x; posit.z = 0;
					//Make cloned object
					//Variable tempClone is used to store object reference to name instantiated objects
					var tempClone = Instantiate(floorTemplate, posit, Quaternion.identity);
					//Name new clone a unique name so it can be referenced when floor must be destroyed
					tempClone.gameObject.name = "ClonedFloor: " + posit.x + " " + posit.y;
				}
			//Destroy old tiles on the opposite side
				for (int x = LRBound[0]; x <= LRBound[1]; x += 1) {
					//Find and destroy floor tile according to its name
					Destroy(GameObject.Find("ClonedFloor: " + x + " " + UDBound[1]));
				}
			//Change upper bound as new blocks have been made
			UDBound[1]--;
		}
	}
	
}
