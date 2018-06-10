using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : PlayerMechanics {

	//How much stamina is taken
	public const int STAMINA_COST = 30;
	//Constants
	public const int DASH_SPEED_MULTIPLIER = 3;
	public const int DASH_LENGTH = 10;

	//Run-time variables
	private MovingObject userScript;
	private bool dashing;
	private Vector2 velocity;
	public int dashLeft=0;

	 //This instance will be used so if both slots are over the same power, they will reference the same power
	 private static Dash instance;

		// Use this for initialization
	//Dash will likely be used by multiple objects; not just by the player. The initialize function gets
	//the moving script of the powerUser
	public override void Initialize (GameObject user) {
		base.Initialize(user);
		userScript = user.GetComponent<MovingObject>();
		
		if(instance == null)
			instance = this;
	}

	void FixedUpdate() {
		if(dashing == true) {
			userScript.Move(velocity*DASH_SPEED_MULTIPLIER);
			if(dashLeft>0) {
				dashLeft--;
			} else {
				EndDash();
			}
		}
	}

	//Start Dash
	//@return Boolean value telling program whether mechanic worked or if Stamina cost or barrier on the map stopped it
	public override bool Activate() {
		velocity = userScript.GetMovement();
		if(velocity != Vector2.zero) {
			bool status = GameManager.instance.DrainStamina(STAMINA_COST);
			if(status) {
				dashing = true;
				dashLeft = DASH_LENGTH;
				userScript.SetMobility(false);
				userScript.AnimateDash(velocity);
				return true;
			}
		}
		//If one or both if statements fail, the code will reach this point and return false
		return false;
	}

	private void EndDash() {
		userScript.SetMobility(true);
		dashing = false;
	}

	public override PlayerMechanics getInstance() {
		return instance;
	}

}
