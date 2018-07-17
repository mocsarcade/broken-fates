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

	//Runtime variables
	public int durability;
	protected int imageNum;

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
			float mySize = shadow.GetSize().x*(3f/2);
			//Take recoil damage
			Damage(damageAmo/mySize);
		}
	}

	//Object's images become more broken as they go through the imageSet array until being officially broken at the last frame
	public override void Damage(float damageAmo) {
		damageAmo = damageAmo/(shadow.GetSize().x*GlobalRegistry.INVERSE_DAMAGE_MULTIPLIER() );
		//Objects with zero or only one image are considered unbreakable and will stay the same forever
		if(imageSet.Length > 1) {
			if(broken == false && imageNum < imageSet.Length) {
				if(damageAmo >= durability) {
					imageNum++;
					myRenderer.sprite = imageSet[imageNum];
					if(imageNum >= imageSet.Length-1) {
						//If Sprite is last possible sprite, it is now the broken image and should be broken
						broken = true;
					}
				}
			}
		}
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
