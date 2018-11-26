using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sneak : PlayerMechanics {

	//How much stamina is taken
	public const int STAMINA_COST = 1;

	 //This instance will be used so if both slots are over the same power, they will reference the same power
	 private static Sneak instance;
	 private bool sneaking;
	 private int timer;

	public override void Initialize (GameObject user) {
		base.Initialize(user);

		if(instance == null)
			instance = this;
	}

	public void Update() {
		if(sneaking) {
			timer++;
			if(timer>2) {
				bool status = GameManager.instance.DrainStamina(STAMINA_COST);
				timer=0;
				//If not enough stamina, stop sneaking
				if(status == false) {
					Release();
				}
			}
		}

	}

	//@return Boolean value telling program whether mechanic worked or if Stamina cost or barrier on the map stopped it
	public override bool Activate() {
		base.Activate();
		powerScript.Sneak();
		sneaking = true;
		//If one or both if statements fail, the code will reach this point and return false
		return true;
	}

	public override void Release() {
		powerScript.WalkNormal();
		sneaking = false;
	}

	public override void Deactivate(bool setMobility) {
		base.Deactivate(setMobility);
		powerScript.WalkNormal();
		sneaking = false;
	}



}
