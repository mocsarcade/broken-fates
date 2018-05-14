using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : PlayerMechanics {

	    //This can be changed in the inspector to change how much stamina is taken by summoning a spell
		public int STAMINA_COST = 100;

		public Vector2 tarPos;
		public GameObject playerObject;

		public GameObject target;
		private GameObject magicTar;

		// Use this for initialization
		void Start () {
			tarPos = transform.position;
			playerObject = GameObject.FindGameObjectWithTag("Player");
		}

		public override bool Activate() {
			if(Inventory.instance.itemsInInventory() > 0) {
				tarPos = playerObject.transform.position;
				//Variable magicTar is used for when magicTar has to be destroyed when the activation key (ex: F) is released
				magicTar = Instantiate(target, tarPos, Quaternion.identity);
				int weight = Inventory.instance.getWeight();
				if(weight>0) {
					magicTar.GetComponent<TargetMovement>().Initialize(tarPos, Inventory.instance.getStrength(), weight);
				}
				//Freeze player so he won't move while target is moving
				Player.getPlayer().SetMobility(false);
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
						Inventory.instance.throwHeldItem((Vector2) playerObject.transform.position, tarPos);
						}
				}
				//Spell cleanup
				Destroy(magicTar);
				Player.getPlayer().SetMobility(true);
		}
	}
}
