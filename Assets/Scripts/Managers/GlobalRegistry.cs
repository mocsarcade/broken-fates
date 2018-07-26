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

}
