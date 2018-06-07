using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : PlayerMechanics {

	//How much stamina is taken
	public const int STAMINA_COST = 35;
	//Constants
	public const int DASH_SPEED_MULTIPLIER = 2;
	public const int DASH_LENGTH = 100;

	//Run-time variables
	private MovingObject userScript;
	private bool dashing;
	private Vector2 velocity;

		// Use this for initialization
	//Dash will likely be used by multiple objects; not just by the player. The initialize function gets
	//the moving script of the powerUser
	public virtual void Initialize (GameObject user) {
		base.Initialize(user);
		userScript = user.GetComponent<MovingObject>();
	}

	void FixedUpdate() {
		if(dashing == true) {
			userScript.Move(velocity*DASH_SPEED_MULTIPLIER);
		}
	}

	//Start Dash
	//@return Boolean value telling program whether mechanic worked or if Stamina cost or barrier on the map stopped it
	public override bool Activate() {
		bool status = GameManager.instance.DrainStamina(STAMINA_COST);
		if(status) {
			dashing = true;
			userScript.SetMobility(false);
			velocity = userScript.GetMovement();
			return true;
		}
		else {
			return false;
		}
	}

}
