using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

using SpecialDungeonRooms;

public class Scene : MonoBehaviour {

	//Optional variables
	public Dialogue[] dialogue;
	public GameObject[] target;
	public SpecialRoom containedRoom;

	void Awake() {
		dialogue = new Dialogue[1];
		dialogue[0] = new Dialogue("Zorlag", new string[] { "Huuuuuuuman! I am Haunting you!", "WhooooOOOh"});
	}

	// When Player enters trigger
	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("Receiving " + other.gameObject.name);
		if(Player.GetPlayer() != null && SceneManager.instance != null) {
			if(other.gameObject == Player.GetPlayer().gameObject) {
				Timing.RunCoroutine(_RunScene());
				Debug.Log("It's a player!");
				//Disable activating this script again
				GetComponent<Collider2D>().enabled = false;
			}
		}
	}

	public virtual IEnumerator<float> _RunScene() {
		SceneManager.instance.SetActivePlay(false);
		//Command 1: Run Dialogue
		yield return Timing.WaitUntilDone(Timing.RunCoroutine(
					SceneManager.instance._RunDialogue(dialogue[0])
					));
		//Command 2: Walk towards
		yield return Timing.WaitUntilDone(Timing.RunCoroutine(
					SceneManager.instance._MoveObjectTo(Player.GetPlayer(), target[0].transform.position)
					));

		SceneManager.instance.CloseDoors(containedRoom);
		SceneManager.instance.SetActivePlay(true);
		Debug.Log("Finishing!");
	}
}
