//NOTE FROM AN EDITOR: When the time comes to add dashing, just change the "speed" varaible from MovingObject. The default speed is 100, and changing that will change the players' speed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The player can onlyexist once, so set this object as a singleton
public class Player : MovingObject
{

	//Singleton varaible
	protected static Player instance;

	public const int WALK_SPEED = 200;

    // animator Gets its component every time this script is "enabled". Basically when the script begins.
    protected override void Awake()
    {
				base.Awake();
				speed = WALK_SPEED;
				//Make Player a Singleton
				if (instance == null)
					instance = this;
				else if (instance != this)
					Destroy(gameObject);
    }

	//In order to Get the player's script, every script must call this Method
	//This gatekeeper can allow additions to how many players/instances we can have
	public static Player GetPlayer() {
		return instance;
	}

	//ComputeVelo sets character's movement speed by keys being pushed. It computes velocity
  protected override void ComputeVelo()
  {
        //Movement method of MovingObject acts ever few ticks to move character according to how calcMovement has been changed. It will be 0 on default
        //Get what keys are being pushed and save into a 2-value vector (Basically just an array with two values).
			calcMovement = (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
   }

	//Renders the player in all scenarios when the player has free movement and is moving
	protected override void UpdateAnimator (Vector2 direction)
	{
		//Set Move and MoveX which tell animator what direction our player should be facing (unless player is idle, in which case direction will be unchanged for animator
		if(direction.magnitude > 0.1 && frozen == false)
		{
			if(animator.GetBool("Moving") == false)
			{
				animator.SetBool("Moving", true);
			}
			animator.SetFloat("MoveY", direction.y);
			animator.SetFloat("MoveX", direction.x);
		}
		else
		{
			if(animator.GetBool("Moving") == true)
			{
				animator.SetBool("Moving", false);
			}
		}
	}

 //Renders the player in all scenarios when the player has free movement and is moving
 public override void AnimateDash (Vector2 direction)
 {
	 //Set Move and MoveX which tell animator what direction our player should be facing (unless player is idle, in which case direction will be unchanged for animator
	 if(direction.magnitude > 0.1)
	 {
		 animator.SetTrigger("Dash");
		 if(animator.GetBool("Moving") == false)
		 {
			 animator.SetBool("Moving", true);
		 }
		 animator.SetFloat("MoveY", direction.y);
		 animator.SetFloat("MoveX", direction.x);
	 }
 }

	protected override void MakeVibration () {
		//Define Ring size so that walking causes a uniform speed, while running or crawling has double the effect
		int ringSize = (int) ((speed*2 - WALK_SPEED)/3)*weight;
		//Create Vibration Ring
		Vibration.Vibrator().MakeVibration(ringSize, GetPosition(), this);
	}

	//When Vibration is felt from other objects
	public override void FeelVibration (Vector2 sourcePosition) {}

	public override void PickUp(GameObject obj) {
		Inventory.instance.PickUp(obj);
	}

	//Interface for throwing so all objects can throw an object if they are holding something.
	public override void throwHeldObject(Vector2 tarPos) {
		Inventory.instance.throwHeldItem(tarPos);
	}

	public Vector2 GetDirection() {
		return new Vector2(animator.GetFloat("MoveX"), animator.GetFloat("MoveY"));
	}

	public override void Damage(float damageAmo) {
		damageAmo = damageAmo/(shadow.GetSize().x*GlobalRegistry.INVERSE_DAMAGE_MULTIPLIER() );
		GameManager.instance.DamageEffect();
		GameManager.instance.DrainCap((int) damageAmo*2);
	}
}
