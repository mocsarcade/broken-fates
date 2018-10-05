using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//TODO: REMOVE THIS. THIS IS JUST FOR TESTING
using DungeonFloor; using DungeonRooms;

public class Loader : MonoBehaviour {

	public GameObject gameManager;
	public Text subNameText;
	public Text subDialogueText;

	//Stuff for testing purposes
	public List<Room> room;

	// Use this for initialization
	void Awake () {
		if (GameManager.instance == null) {
			Instantiate (gameManager);

			//Test FloorMaker
			Floor testFloor = new Floor(9,9,room);
			RoomManager.instance.createFloor(testFloor);
			//gameController.GetComponent<DialogueManager>().nameText = subNameText;
			//gameController.GetComponent<DialogueManager>().dialogueText = subDialogueText;
		}
	}
}
