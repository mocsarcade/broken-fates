using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MechanicsManager : MonoBehaviour {

	//This is a field, a special quirk of C#. This field keeps the number of slots
	//The player has and is read-only
	private int _mechanic_slots = 2;
	public int MECHANIC_SLOTS {
		get{return _mechanic_slots;}
	}

	//Set up array of all mechanics to place into keyMechanics when any mechanic is set by player
	//NOTE: LATER MECHANICOBJECTS WILL BE FILLED BY THE SAVE FILE. MECHANICOBJECTS HOLDS THE OBJECTS THE PLAYER
	//CAN CURRENTLY USE. THIS WILL GROW AS THE GAME CONTINUES AND SO THIS ARRAY MUST BE SAVED AND LOADED
	public PlayerMechanics[] mechanicObjects;
	//This is an the actual array of mechanics that the player has equipped.
	public PlayerMechanics[] keyMechanics;
	//The array of UI images for the player's equipped mechanics
	public Image[] UISlots;

	//Run-time variables
	private bool[] mechanicMenu;
	private bool[] axisPushed;
	private int[] currentPower;
	//This variable keeps track of the current changeSize method and stops it if the user is impatient
	private Coroutine[] runningRoutine;


	// Use this for initialization
	void Awake () {
		//Place an instantiated (constructed) object for each prefab in its own array
		//This will be called to change keyMechanics
		keyMechanics = new PlayerMechanics[MECHANIC_SLOTS];
		//Define other arrays
		mechanicMenu = new bool[MECHANIC_SLOTS];
		UISlots = new Image[MECHANIC_SLOTS];
		currentPower = new int[MECHANIC_SLOTS];
		runningRoutine = new Coroutine[MECHANIC_SLOTS];
		axisPushed = new bool[MECHANIC_SLOTS];
		GameObject MechanicsBox = GameObject.FindWithTag("MechanicsBox");
		for (int slot=0; slot<MECHANIC_SLOTS; slot++) {
			//Fill UISlots
			UISlots[slot] = MechanicsBox.GetComponent<RectTransform>().Find("Mechanic " + (slot+1)).GetComponent<Image>();
			//Fill keyMechanics
			setKeyMechanic(slot, slot);
		}
	}

	// Update is called once per frame
	void Update () {
		//Check the STATIC OBJECT ARRAY, which is an array of PowerSlot Objects, and if either 0, 1 or 2 has a keycode that is being
		//pressed, get its power attached and do it.
			for (int i=0; i < MECHANIC_SLOTS; i++) {
				//Open a Power-choosing menu
				if(Input.GetButtonDown ("PowerMenu" + i)) {
					mechanicMenu[i] = true;
					//If the Image is currently shrinking, stop it from doing that!
	        if(runningRoutine[i] != null) {
	          StopCoroutine(runningRoutine[i]);
	        }
					runningRoutine[i] = StartCoroutine(resizeUIImage(UISlots[i], 75));
					Player.getPlayer().SetMobility(false);
				}
				if(Input.GetButtonUp ("PowerMenu" + i)) {
					mechanicMenu[i] = false;
					//If the Image is currently growing, stop it from doing that!
	        if(runningRoutine[i] != null) {
	          StopCoroutine(runningRoutine[i]);
	        }
					runningRoutine[i] = StartCoroutine(resizeUIImage(UISlots[i], 50));
					//If all other mechanicMenu variables are false as well, reactivate mobility
					//Start by reactivating mobility...
					Player.getPlayer().SetMobility(true);
					for (int j=0; j < MECHANIC_SLOTS; j++) {
						//...And if any other mechanicMenu variables ARE true, reset it to false!
						if(mechanicMenu[j] == true) {
							Player.getPlayer().SetMobility(false);
						}
					}
				}
				//Activating a Power
				if(mechanicMenu[i] == false) {
					activePower(i);
				} else {
					activeMenu(i);
				}
			}
	}

	//Activemenu keeps track of whether the horizontal button is being pushed to make the respective
	//power in the menu to toggle sideways.
	private void activeMenu(int index) {
		//Get whether the player is pressing left or right (in integer form)
		int pushedDirection = (int) Input.GetAxisRaw("Horizontal");
		if(pushedDirection !=0 && axisPushed[index]==false) {
			//Toggle power left or right depending on whether pushedDirection is 1 or -1
			int newPower = currentPower[index] + pushedDirection;
			//Make sure the toggle of the power isn't out of bounds
			if(newPower >= mechanicObjects.Length) {
				newPower = 0;
			} else if(newPower < 0) {
				newPower = mechanicObjects.Length-1;
			}
			//Set the mechanic to change now
			setKeyMechanic(index, newPower);
			//Change axisPushed so that the menu won't keep changing every frame
			axisPushed[index] = true;
		} else if(pushedDirection == 0 && axisPushed[index]==true) {
			//Once the player stops pressing the left or right button, program becomes ready to accept left/right input again
			axisPushed[index] = false;
		}
	}

	private void activePower(int index) {
		if (Input.GetButtonDown ("Mechanic" + index)) {
			keyMechanics[index].Activate();
		}
		if (Input.GetButtonUp ("Mechanic" + index)) {
			keyMechanics[index].Release();
		}
	}

	// Enlarge the mechanics icon when the menu button is held, so it is clear which menu is being shifted
	private IEnumerator resizeUIImage(Image mechanicIcon, int newSize) {
		RectTransform iconTransform = mechanicIcon.gameObject.GetComponent<RectTransform>();
		while(Mathf.Abs(iconTransform.sizeDelta.x-newSize) > 1) {
			if(iconTransform.sizeDelta.x > newSize) {
				iconTransform.sizeDelta = new Vector2(iconTransform.sizeDelta.x-1, iconTransform.sizeDelta.y-1);
			} else {
				iconTransform.sizeDelta = new Vector2(iconTransform.sizeDelta.x+1, iconTransform.sizeDelta.y+1);
			}
			yield return null;
		}
	}

	private void setKeyMechanic(int keyID, int newPowerSlot) {
		//Destroy old mechanic object and create new one
		/*if(keyMechanics[keyID] != null) {
			Destroy(keyMechanics[keyID]);
		}*/
		currentPower[keyID] = newPowerSlot;
		if(mechanicObjects[newPowerSlot].getInstance() == null) {
			keyMechanics[keyID] = Instantiate(mechanicObjects[newPowerSlot]);
			keyMechanics[keyID].GetComponent<PlayerMechanics>().Initialize (Player.getPlayer().gameObject);
		} else {
			keyMechanics[keyID] = mechanicObjects[newPowerSlot].getInstance();
		}
		//Update UI
		UISlots[keyID].sprite = mechanicObjects[newPowerSlot].getIcon();
	}

}
