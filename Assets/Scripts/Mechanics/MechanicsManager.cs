using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Types
{
	public enum Mechanics {
		TimeBlink, 
		Dash, 
		Stone
	}


	public class MechanicsManager : MonoBehaviour {

		KeyCode[] keys;
		GameObject player;

	// Use this for initialization
	void Start () {
			keys = new KeyCode[] { KeyCode.A, KeyCode.S, KeyCode.D };
			GameManager.keyMechanics = new Mechanics[] {Mechanics.TimeBlink, Mechanics.Dash, Mechanics.Stone};
			player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		//Check the STATIC OBJECT ARRAY, which is an array of PowerSlot Objects, and if either 0, 1 or 2 has a keycode that is being
		//pressed, get its power attached and do it.
			if(Input.GetKeyDown(KeyCode.Space))
			{
				//Sneak
			}
			if(Input.GetKeyUp(KeyCode.Space))
			{
				//Stop Sneaking
			}

			if(Input.GetKeyDown(KeyCode.LeftShift))
			{
				//Run
				player.GetComponent<PlayerMovement>().run();
			}
			if(Input.GetKeyUp(KeyCode.LeftShift))
			{
				//Stop Running
				player.GetComponent<PlayerMovement>().walk();
			}

			for (int i=0; i < 3; i++) {
				if (Input.GetKeyDown (keys [i])) {
					switch (GameManager.keyMechanics [i]) {
					case Mechanics.TimeBlink:
						Debug.Log ("Case 1");
						break;
					case Mechanics.Dash:
						Debug.Log ("Case 2");
						break;
					case Mechanics.Stone:
						Debug.Log ("Case 3");
						break;
					}
				}

			}
	}
}

}