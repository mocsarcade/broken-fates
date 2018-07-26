using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using MEC;

public class GameManager : MonoBehaviour {

	//Important asset references used by the entire program:
	public static GameManager instance = null;
	public Data[] dataReference;
	public GameObject MementoType;

	//Player's max stamina will increase as the player Gets upgrades. This variable is the current max
	private int CURRENT_MAX_STAMINA = 200;
	private const float STAMINA_REGEN_RATE = 0.25f;
	private const float STAMINA_CAP_DRAIN_RATE = 0.01f;
	private int STAMINA_BOX_WIDTH = 125;

	//Player-damage variables
	private const float SHAKE_DELAY = 0.05f;
	private const int DAMAGE_OPAQUE_LEVEL = 100;
	private const float DAMAGE_RED = 173f/255;
	private const float RED_VIEW_DELAY = 0.005f;

	//Run-time variables
	private int StaminaCap;
	private int Stamina;

	//Stamian Bar Slider (Holds a float from 0 to 1)
 	private Slider staminaBarSlider;  //reference for slider
	private RectTransform staminaBarObject;  //reference for slider
	public Image ScreenOverlay; //Reference for screen Image effect

	// Use this for initialization
	void Awake () {
		//Make GameManager a Singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	}

	void Start() {
		//Load staminaBarSlider object
		GameObject sliderBarObject = GameObject.FindWithTag("StaminaBar");
		if(sliderBarObject) {
			staminaBarSlider = sliderBarObject.GetComponent<Slider>();
			staminaBarObject = (RectTransform) sliderBarObject.GetComponent<RectTransform>().parent;
			StaminaCap = CURRENT_MAX_STAMINA;
			Stamina = CURRENT_MAX_STAMINA;
		}

		//Load ScreenTexture Image
		GameObject ScreenTexture = GameObject.FindWithTag("ScreenTexture");
		if(ScreenTexture) {
			ScreenOverlay = ScreenTexture.GetComponent<Image>();
		}

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
			Stamina = StaminaCap;
		}
		UpdateStamina();
	}

	public int GetStamina() {
		return Stamina;
	}

	public int GetCap() {
		return StaminaCap;
	}

	public void setCap(int newCap) {
		Stamina += (newCap - StaminaCap);
		StaminaCap = newCap;
		UpdateStamina();
	}

  //A large shrink (such as for QuickLife) will require the stamina to drop slowly. This provides that effect
	public bool DrainCap(int drainRate) {
		if(drainRate > 0) {
			if(StaminaCap >= drainRate) {
				Timing.RunCoroutine(SlowDrain(drainRate));
				return true;
			} else {
				//If there isn't enough stamina available, swap worlds
				swapWorld();
				return false;
			}
		} else {
			Debug.LogException(new Exception("Program is trying to drain a negative number!"), this);
			return false;
		}
	}

  //A large shrink (such as for QuickLife) will require the stamina to drop slowly. This provides that effect
	private IEnumerator<float> SlowDrain(int drainAmo) {
		int drainLeft = drainAmo;
		while(drainLeft > 0) {
		if(StaminaCap >= 1) {
			StaminaCap -= 1;
		if(Stamina > StaminaCap)
			Stamina = StaminaCap;
		UpdateStamina();
		} else {
			//If there isn't enough stamina available, swap worlds
			swapWorld();
		}
		drainLeft--;
		yield return Timing.WaitForOneFrame;
		}
	}

  //If the Player loads a save or uses a potion, his stamina cap will increase
	public void RegenCap(int amount) {
		if(amount > 0) {
			if(StaminaCap+amount <= CURRENT_MAX_STAMINA) {
				StaminaCap += amount;
			} else {
				StaminaCap = CURRENT_MAX_STAMINA;
			}
			//Increase the player's stamina to match
			Stamina += amount;
			UpdateStamina();
		}
	}

	public void DamageEffect() {
		Timing.RunCoroutine(CameraShake().CancelWith(Camera.main.gameObject));
		Timing.RunCoroutine(RedView(100).CancelWith(ScreenOverlay.gameObject));
	}

	private IEnumerator<float> CameraShake() {
		CameraController currentCamera = Camera.main.gameObject.GetComponent<CameraController>();
		for(int amount=5; amount>0; amount--) {
			for(int direc=1; direc>=-1; direc=direc-2) {
				currentCamera.Shake(amount*direc);
				yield return Timing.WaitForSeconds(SHAKE_DELAY);
			}
		}
	}

	private IEnumerator<float> RedView(int startingOpacity) {
		int opacity = startingOpacity;
		while(opacity>0) {
			ScreenOverlay.color = new Color(DAMAGE_RED,0,0,opacity/255f);
			opacity--;
			yield return Timing.WaitForSeconds(RED_VIEW_DELAY);
		}
	}

  //Method that swaps world whenever the player runs out of stamina
	//This method may be moved to its own class as the internals of this method might Get too complicated
	private void swapWorld() {
		//Code to be written later
	}

	public Data GetDataReference(GameManager.DataType returnType) {
		switch(returnType) {
			case GameManager.DataType.t_AnimatorData:
				return dataReference[0];
			case GameManager.DataType.t_MementoData:
				return dataReference[1];
		}
		//If none of the cases were fulfilled, raise an error and return null
		Debug.LogException(new Exception("Trying to Get Data Reference of a returnType that isn't registered: " + returnType));
		return null;
	}

	public void SaveGameStatistics(QuickLife script) {
		GameMemento newMemento = Instantiate(MementoType).GetComponent<GameMemento>();
		newMemento.InitializeGame(this);
		script.SaveMemento(newMemento);
	}

	//This method is used as the base for reverting the hand_index coroutine.
	public void RevertHand(int handIndex) {
		Timing.RunCoroutine(Revert_handindex(handIndex));
	}
	//coroutine to set the players' hand to the initial value at the time of saving
	//This method is called by the GameManager, because the GameMemento is destroyed a few frames after
	public IEnumerator<float> Revert_handindex(int handIndex) {
		int inventoryCount = Inventory.instance.itemsInInventory();
		int curHandIndex = Inventory.instance.GetInventoryIndex();
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
			curHandIndex = Inventory.instance.GetInventoryIndex();
			if(handIndex != curHandIndex) {
			}
			yield return Timing.WaitForOneFrame;
		} while (handIndex != curHandIndex);
	}

	public enum DataType {
		t_AnimatorData,
		t_MementoData
	}
}
