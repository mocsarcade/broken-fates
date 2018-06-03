using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicsManager : MonoBehaviour {

	//This is a field, a special quirk of C#. This field keeps the number of slots
	//The player has and is read-only
	private int _mechanic_slots = 2;
	public int MECHANIC_SLOTS {
		get{return _mechanic_slots;}
	}

	//Set up array of all mechanics to place into keyMechanics when any mechanic is set by player
	public PlayerMechanics[] mechanicObjects;
	//This is an the actual array of mechanics that the player has equipped
	public PlayerMechanics[] keyMechanics;


	// Use this for initialization
	void Awake () {
		//Place an instantiated (constructed) object for each prefab in its own array
		//This will be called to change keyMechanics
		keyMechanics = new PlayerMechanics[MECHANIC_SLOTS];
		for (int slot=0; slot<MECHANIC_SLOTS; slot++) {
			keyMechanics[slot] = Instantiate(mechanicObjects[slot]);
			keyMechanics[slot].GetComponent<PlayerMechanics>().Initialize (Player.getPlayer().gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		//Check the STATIC OBJECT ARRAY, which is an array of PowerSlot Objects, and if either 0, 1 or 2 has a keycode that is being
		//pressed, get its power attached and do it.
			if(Input.GetButtonDown("Sneak"))
			{
				//Sneak
			}
			if(Input.GetButtonUp("Sneak"))
			{
				//Stop Sneaking
			}

			if(Input.GetButtonDown("Run"))
			{
				//Run
				Player.getPlayer().run();
			}
			if(Input.GetButtonUp("Run"))
			{
				//Stop Running
				Player.getPlayer().walk();
			}

			for (int i=0; i < MECHANIC_SLOTS; i++) {
				if (Input.GetButtonDown ("Mechanic" + i)) {
					keyMechanics[i].Activate();
				}
				if (Input.GetButtonUp ("Mechanic" + i)) {
					keyMechanics[i].Release();
				}
			}
	}

}
