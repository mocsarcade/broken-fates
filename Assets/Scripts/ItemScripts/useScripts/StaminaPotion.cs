using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPotion : ItemScript {

	public int gainAmount;

	public override bool Use () {
		//Increase max stamina
		GameManager.instance.RegenCap(gainAmount);
		//Potions will be destroyed upon use
		return true;
	}
}
