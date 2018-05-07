using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicsManager : MonoBehaviour {

	KeyCode[] keys;
	GameObject player;

	// Use this for initialization
	void Start () {
			keys = new KeyCode[] { KeyCode.A, KeyCode.S, KeyCode.D };
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

			for (int i=0; i < GameManager.instance.MECHANIC_SLOTS; i++) {
				if (Input.GetKeyDown (keys [i])) {
					GameManager.instance.getMechanic(i).Activate();
				}
				if (Input.GetKeyUp (keys [i])) {
					GameManager.instance.getMechanic(i).Release();
				}
			}
	}

}
