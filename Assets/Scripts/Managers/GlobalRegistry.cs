using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalRegistry : MonoBehaviour {

	private static GlobalRegistry instance = null;

	void Awake () {
		//Make GameManager a Singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	}

	private float _INVERSE_DAMAGE_MULTIPLIER = 2f;
	public static float INVERSE_DAMAGE_MULTIPLIER() {
		//Call private method to get Inverse Damage Multiplier
		return instance.Get_Inverse_Damage_Multiplier();
	}
	//Private method used by public GET method
	private float Get_Inverse_Damage_Multiplier() {return _INVERSE_DAMAGE_MULTIPLIER;}


	private int _SORTING_Y_MULTIPLIER = -3;
	public static int SORTING_Y_MULTIPLIER() {
		//Call private method to get Multiplier for Y to get the object's sorting order
		return instance.Get_Sorting_Y_Multiplier();
	}
	//Private method used by public GET method
	private int Get_Sorting_Y_Multiplier() {return _SORTING_Y_MULTIPLIER;}


	private const float _PLAYER_REACH = 0.75f;
	public static float PLAYER_REACH() {
		//Call private method to get Reach
		return instance.Get_Player_Reach();
	}
	//Private method used by public GET method
	private float Get_Player_Reach() {return _PLAYER_REACH;}

}
