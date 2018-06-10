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
			userScript = user.GetComponent<MovingObject>();
			tarPos = transform.position;
			
			if(instance == null)
				instance = this;
		}

		public override bool Activate() {
			if(Inventory.instance.itemsInInventory() > 0) {
				tarPos = powerUser.transform.position;
				//Variable magicTar is used for when magicTar has to be destroyed when the activation key (ex: F) is released
				magicTar = Instantiate(target, tarPos, Quaternion.identity);
				int weight = Inventory.instance.getWeight();
				if(weight>0) {
					magicTar.GetComponent<TargetMovement>().Initialize(tarPos, Inventory.instance.getStrength(), weight);
				}
				//Freeze user so he/she won't move while target is moving
				userScript.SetMobility(false);
				return true;
			} else {
				return false;
			}
		}

		public override void Release () {
			if(Inventory.instance.itemsInInventory() > 0) {
				if((Vector2) magicTar.transform.position != tarPos) {
					//Drain stamina for using spell. Status is the status variable for whether player had enough stamina for activation
					bool status = GameManager.instance.DrainStamina(STAMINA_COST);
					if(status == true) {
						//Find target's position so rock can be created there
						tarPos = magicTar.transform.position;
						userScript.throwHeldObject(tarPos);
					}
				}
				//Spell cleanup
				Destroy(magicTar);
				userScript.SetMobility(true);
		}
	}

	public override PlayerMechanics getInstance() {
		return instance;
	}
}
