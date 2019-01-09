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
		yield return 0;
	}
}
