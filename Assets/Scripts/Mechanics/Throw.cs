using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : PlayerMechanics {

	   //How much stamina is taken by summoning a spell
		public const int STAMINA_COST = 15;

		public Vector2 tarPos;
		private MovingObject userScript;

		public GameObject target;
		private GameObject magicTar;

		 //This instance will be used so if both slots are over the same power, they will reference the same power
		 private static Throw instance;

		// Use this for initialization
		public override void Initialize (GameObject user) {
			base.Initialize(user);
			userScript = powerUser.GetComponent<MovingObject>();
			tarPos = transform.position;

			if(instance == null)
				instance = this;
		}

		public override bool Activate() {
			base.Activate();
			if(Inventory.instance.isHolding()) {
				tarPos = powerUser.transform.position;
				//Variable magicTar is used for when magicTar has to be destroyed when the activation key (ex: F) is released
				magicTar = Instantiate(target, tarPos, Quaternion.identity);
				int weight = Inventory.instance.GetWeight();
				if(weight>0) {
					magicTar.GetComponent<TargetMovement>().Initialize(tarPos, Inventory.instance.GetStrength(), weight);
				}
				//Freeze user so he/she won't move while target is moving
				userScript.SetMobility(false);
				return true;
			} else {
				return false;
			}
		}

		public override void Release () {
			base.Release();
			if(Inventory.instance.isHolding()) {
				if((Vector2) magicTar.transform.position != tarPos) {
					//Drain stamina for using spell. Status is the status variable for whether player had enough stamina for activation
					bool status = GameManager.instance.DrainStamina(STAMINA_COST);
					if(status == true) {
						//Find target's position so rock can be created there
						tarPos = magicTar.transform.position;
						userScript.throwHeldObject(tarPos);
					}
				}
		}
		if(magicTar) {
			//Spell cleanup
			Destroy(magicTar);
			userScript.SetMobility(true);
		}
	}

	//When power is swapped prematurely before power ends, this method is called to clean up the power
	public override void Deactivate(bool setMobility)
	{
		base.Deactivate(setMobility);
		//Spell cleanup
		Destroy(magicTar);
	}

	public override PlayerMechanics GetInstance() {
		return instance;
	}
}
