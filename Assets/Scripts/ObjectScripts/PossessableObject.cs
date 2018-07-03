using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessableObject : Material {

	//Power of ghost possessing object, set by inspector or by ghost when it takes over the object
	//public float strength;
	//public bool isPossessed;
	public Ghost possessed;
	public bool broken;
	public Sprite brokenImage;

	public override void FeelVibration(Vector2 sourcePosition) {
		if(possessed && beingThrown == false && broken == false) {
			Vector2 newPosition = (sourcePosition*2) - (Vector2) transform.position;

			StartCoroutine(Throw(newPosition, possessed.getStrength()));
		}
	}

	/*
	public override IEnumerator Throw(Vector2 target, float strength) {
		beingThrown = true;
		bool curState;
		IEnumerator baseThrow = base.Throw(target, strength);
		do {
			curState = baseThrow.MoveNext();
			yield return null;
		} while(curState);
		beingThrown = false;
	}*/

	//Do damage if this object was being thrown
	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject != thrower) {
			base.OnCollisionEnter2D(collision);
			if(beingThrown) {
				//GetComponent will return null if the object does not have a material script
				Material hitObject = collision.gameObject.GetComponent<Material>();
				if(hitObject) {
					hitObject.Damage(weight*possessed.getStrength());
				}
				Damage(weight*possessed.getStrength());
			}
		}
	}

	//This superclass possessed object is the simplest: It is broken instantly upon use
	//NOTE: THE METHOD THAT AFFECTS DURABILITY MAY BE ENCAPSULATED INTO A STRATEGY CLASS WITH
	//A DIFFERENT OBJECT FOR EACH DAMAGE TYPE. THIS WILL MINIMIZE SUBCLASSING
	public override void Damage(float damage) {
		broken = true;
		myRenderer.sprite = brokenImage;
	}

	//The second half of time blink. This method uses the object to reconstruct the object into its old form
	public override void useMemento(Memento oldState) {
		//If Image is being updated to former image, update the broken tag
		if(myRenderer.sprite != oldState.sprite) {
			if(oldState.sprite == brokenImage) {
				broken = false;
			} else {
				broken = true;
			}
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
