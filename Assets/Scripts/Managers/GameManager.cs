using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	//Important asset references used by the entire program:
	public static GameManager instance = null;
	public AnimatorData dataReference;
	public GameObject MementoType;

	//Run-time variables
	public const int MAX_STAMINA = 200000;
	private int StaminaCap = MAX_STAMINA;
	private int Stamina = MAX_STAMINA;

	// Use this for initialization
	void Awake () {
		//Make GameManager a Singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		LoadMemento();
		DontDestroyOnLoad (gameObject);
	}

	private void LoadMemento() {
		MementoType = (GameObject) Resources.Load("GameMemento");
	}

	// Update is called once per frame. Will be used later.
	//void FixedUpdate () {
	//}

  //Method to drain stamina for every mechanic the player uses. Running and other acrobatics also use DrainStamina
	//@return Boolean value to show whether there was enough stamina available
	public bool DrainStamina(int cost) {
		//If a mechanic needs stamina, it won't run unless there is enough stamina available
		if(Stamina > cost) {
			Stamina -= cost;
			return true;
		}
		else {
			//If there isn't enough stamina available, swap worlds
			swapWorld();
			return false;
		}
	}

  //Stamina will be slowly regained up to the Stamina's Cap. This method will be called by the timer in the gameManager's
	//"Update" method
	public void RegenStamina(int gainRate) {
		Stamina += gainRate;
		if(Stamina > StaminaCap) {
			Stamina = StaminaCap;
		}
	}

	public int getStamina() {
		return Stamina;
	}

  //Over time, the Player's StaminaCap will deplete, forcing the player to switch worlds because the stamina bar's max will
	//Shrink to a size too small to keep playing with
	public void DrainCap(int drainRate) {
		StaminaCap -= drainRate;
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
				Inventory.instance.toggleHandRight();
			} else {
				Inventory.instance.toggleHandLeft();
			}
			curHandIndex = Inventory.instance.getInventoryIndex();
			Debug.Log("Current index is " + curHandIndex + ". The goal is: " + handIndex);
			if(handIndex != curHandIndex) {
				Debug.Log("We're still going!");
			}
			yield return new WaitForFixedUpdate();
		} while (handIndex != curHandIndex);
		Debug.Log("It's over!");
	}
}
