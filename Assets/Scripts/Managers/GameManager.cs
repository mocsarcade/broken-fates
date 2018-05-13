using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public const int MAX_STAMINA = 200000;
	public static GameManager instance = null;
	private int StaminaCap = MAX_STAMINA;
	private int Stamina = MAX_STAMINA;

	// Use this for initialization
	void Awake () {
		//Make GameManager a Singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad (gameObject);
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
}
