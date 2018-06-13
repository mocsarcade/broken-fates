using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

	//Important asset references used by the entire program:
	public static GameManager instance = null;
	public AnimatorData dataReference;
	public GameObject MementoType;

	//Player's max stamina will increase as the player gets upgrades. This variable is the current max
	private int CURRENT_MAX_STAMINA = 200;
	private const float STAMINA_REGEN_RATE = 0.25f;
	private const float STAMINA_CAP_DRAIN_RATE = 0.01f;
	private int STAMINA_BOX_WIDTH = 125;

	//Run-time variables
	private float StaminaCap;
	private int Stamina;

	//Stamian Bar Slider (Holds a float from 0 to 1)
 	private Slider staminaBarSlider;  //reference for slider
	private RectTransform staminaBarObject;  //reference for slider

	// Use this for initialization
	void Awake () {
		//Make GameManager a Singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		//Load staminaBarSlider object
		GameObject sliderBarObject = GameObject.FindWithTag("StaminaBar");
		staminaBarSlider = sliderBarObject.GetComponent<Slider>();
		staminaBarObject = (RectTransform) sliderBarObject.GetComponent<RectTransform>().parent;
		StaminaCap = CURRENT_MAX_STAMINA;
		Stamina = CURRENT_MAX_STAMINA;

		LoadMemento();
		DontDestroyOnLoad (gameObject);
	}

	//Declare variables made for only keeping track of the FixedUpdate
	private int regenCount; private int drainCount;
	//FixedUpdate regenerates the players' stamina and very slowly drains the max stamina cap
	void FixedUpdate() {
		//Regenerate Stamina
		if(Stamina < StaminaCap) {
			regenCount++;
			if(regenCount >= (1/STAMINA_REGEN_RATE)) {
				RegenStamina(1);
				regenCount = 0;
			}
		}
		//Drain Stamina Cap
		drainCount++;
		if(drainCount >= (1/STAMINA_CAP_DRAIN_RATE)) {
			DrainCap(1);
			drainCount=0;
		}
	}

	private void LoadMemento() {
		MementoType = (GameObject) Resources.Load("GameMemento");
	}

	private void UpdateStamina() {
		staminaBarSlider.value = (float) Stamina/StaminaCap;
		Vector2 curSizeDelta = staminaBarObject.sizeDelta;
		staminaBarObject.sizeDelta = new Vector2((float) STAMINA_BOX_WIDTH*StaminaCap/CURRENT_MAX_STAMINA, curSizeDelta.y);
		Vector2 curPosition = staminaBarObject.anchoredPosition;
		staminaBarObject.anchoredPosition = new Vector2(((float) STAMINA_BOX_WIDTH*StaminaCap/CURRENT_MAX_STAMINA)/2, curPosition.y);
	}

  //Method to drain stamina for every mechanic the player uses. Running and other acrobatics also use DrainStamina
	//@return Boolean value to show whether there was enough stamina available
	public bool DrainStamina(int cost) {
		//If a mechanic needs stamina, it won't run unless there is enough stamina available
		if(Stamina >= cost) {
			Stamina -= cost;
			UpdateStamina();
			return true;
		}
		else {
			//If there isn't enough stamina available, swap worlds
			swapWorld();
			UpdateStamina();
			return false;
		}
	}

  //Stamina will be slowly regained up to the Stamina's Cap. This method will be called by the timer in the gameManager's
	//"Update" method
	public void RegenStamina(int gainRate) {
		Stamina += gainRate;
		if(Stamina > StaminaCap) {
			Stamina = (int) StaminaCap;
		}
		UpdateStamina();
	}

	public int getStamina() {
		return Stamina;
	}

  //Over time, the Player's StaminaCap will deplete, forcing the player to switch worlds because the stamina bar's max will
	//Shrink to a size too small to keep playing with
	public bool DrainCap(int drainRate) {
		if(drainRate > 0) {
			if(StaminaCap >= drainRate) {
				StaminaCap -= drainRate;
				if(Stamina > StaminaCap) {
					Stamina = (int) StaminaCap;
				}
				UpdateStamina();
				return true;
			} else {
				//If there isn't enough stamina available, swap worlds
				swapWorld();
				UpdateStamina();
				return false;
			}
		} else {
			Debug.LogException(new Exception("Program is trying to drain a negative number!"), this);
			return false;
		}
	}

  //Over time, the Player's StaminaCap will deplete, forcing the player to switch worlds because the stamina bar's max will
	//Shrink to a size too small to keep playing with
	public void RegenCap(int amount) {
		if(amount > 0) {
			if(StaminaCap+amount <= CURRENT_MAX_STAMINA) {
				StaminaCap += amount;
			} else {
				StaminaCap = CURRENT_MAX_STAMINA;
			}
			Stamina += amount;
			UpdateStamina();
		}
	}

  //Method that swaps world whenever the player runs out of stamina
	//This method may be moved to its own class as the internals of this method might get too complicated
	private void swapWorld() {
		//Code to be written later
	}

	public AnimatorData getDataReference() {
		return dataReference;
	}

	public void SaveGameStatistics(QuickLife script) {
		GameMemento newMemento = Instantiate(MementoType).GetComponent<GameMemento>();
		newMemento.InitializeGame(this);
		script.SaveMemento(newMemento);
	}

	//This method is used as the base for reverting the hand_index coroutine.
	public void RevertHand(int handIndex) {
		StartCoroutine(Revert_handindex(handIndex));
	}
	//coroutine to set the players' hand to the initial value at the time of saving
	//This method is called by the GameManager, because the GameMemento is destroyed a few frames after
	public IEnumerator Revert_handindex(int handIndex) {
		int inventoryCount = Inventory.instance.itemsInInventory();
		int curHandIndex = Inventory.instance.getInventoryIndex();
		//False equates to left, while true equates to right
		bool direction = true;
		do {
			//Calculate direction by which variable is larger
			if(curHandIndex>handIndex) {
				direction = false;
			} else {
				direction = true;
			}
			//Calculates whether moving across the list or looping around the beginning/end is faster
			if(Mathf.Max(handIndex, curHandIndex)-Mathf.Min(handIndex, curHandIndex) >= inventoryCount-Mathf.Max(handIndex, curHandIndex)+Mathf.Min(handIndex, curHandIndex)) {
				direction = !direction;
			}
			if(direction == true) {
				Inventory.instance.toggleHand(1);
			} else {
				Inventory.instance.toggleHand(-1);
			}
			curHandIndex = Inventory.instance.getInventoryIndex();
			if(handIndex != curHandIndex) {
			}
			yield return new WaitForFixedUpdate();
		} while (handIndex != curHandIndex);
	}
}
