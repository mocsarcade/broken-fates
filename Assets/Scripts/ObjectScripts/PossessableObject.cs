using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessableObject : Material {

	//Power of ghost possessing object, set by inspector or by ghost when it takes over the object
	//public float strength;
	//public bool isPossessed;
	public Ghost possessed;
	protected bool broken;
	public Sprite[] imageSet;
	public DamageStrategy strategy;

	public override void FeelVibration(Vector2 sourcePosition) {
		if(possessed && beingThrown == false && broken == false) {
			Vector2 newPosition = (sourcePosition*2) - (Vector2) GetPosition();

			StartCoroutine(Throw(newPosition, 15));//possessed.GetStrength()));
		}
	}

	/*
	public override IEnumerator Throw(Vector2 tarGet, float strength) {
		beingThrown = true;
		bool curState;
		IEnumerator baseThrow = base.Throw(tarGet, strength);
		do {
			curState = baseThrow.MoveNext();
			yield return null;
		} while(curState);
		beingThrown = false;
	}*/

	public override float GetDamageAmount() {
		if(beingThrown) {
			return weight*possessed.GetStrength();
		} else {
			return 0;
		}
	}

	//Attack function all objects use to damage other objects. Is the only function that calls the damage function
	public override void Attack(Material damaged, float damageAmo) {
		base.Attack(damaged, damageAmo);
		if(damageAmo > 0) {
			//Take recoil damage
			Damage(damageAmo/2);
		}
	}

	//This superclass possessed object is the simplest: It is broken instantly upon use
	//NOTE: THE METHOD THAT AFFECTS DURABILITY MAY BE ENCAPSULATED INTO A STRATEGY CLASS WITH
	//A DIFFERENT OBJECT FOR EACH DAMAGE TYPE. THIS WILL MINIMIZE SUBCLASSING. CONSIDER LATER
	public override void Damage(float damageAmo) {
		/*
		if(broken == false && imageSet.Length > 0) {
			//Strategy object takes damage and uses it to change object's sprite
			strategy.Damage(damageAmo, myRenderer.sprite)
			if(myRenderer.sprite == imageSet[imageSet.Length-1]) {
				//If Sprite is last possible sprite, it is now the broken image and should be broken
				broken = true;
			}
		}
		*/
	}

	//The second half of time blink. This method uses the object to reconstruct the object into its old form
	public override void useMemento(Memento oldState) {
		//If Image is being updated to former image, update the broken tag
		if(myRenderer.sprite != oldState.sprite) {
			if(oldState.sprite == imageSet[imageSet.Length-1]) {
				broken = true;
			} else {
				broken = false;
			}
			myRenderer.sprite = oldState.sprite;
		}
		base.useMemento(oldState);
	}

	public bool Possess(Ghost possessor) {
		if(possessed == null) {
			possessed = possessor;
			return true;
		} else {
			//If this object is already occupied, return false to let the ghost know
			return false;
		}
	}
}
