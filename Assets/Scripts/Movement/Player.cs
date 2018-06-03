//NOTE FROM AN EDITOR: When the time comes to add dashing, just change the "speed" varaible from MovingObject. The default speed is 100, and changing that will change the players' speed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The player can onlyexist once, so set this object as a singleton
public class Player : MovingObject
{

	//Singleton varaible
	protected static Player instance;

	private Animator animator;
	public const int WALK_SPEED = 100;
	public const int RUN_SPEED = 150;
	public const int INVERSE_RING_SIZE = 4;

	//offset Vibrations are placed at
	private readonly Vector2 FEET_POSITION = new Vector2(0,-0.6f);

    // animator gets its component every time this script is "enabled". Basically when the script begins.
    protected override void Awake()
    {
				base.Awake();
        animator = GetComponent<Animator>();
				speed = WALK_SPEED;
				//Make Player a Singleton
				if (instance == null)
					instance = this;
				else if (instance != this)
					Destroy(gameObject);
    }

	//In order to get the player's script, every script must call this Method
	//This gatekeeper can allow additions to how many players/instances we can have
	public static Player getPlayer() {
		return instance;
	}

	//ComputeVelo sets character's movement speed by keys being pushed. It computes velocity
  protected override void ComputeVelo()
  {
        //Movement method of MovingObject acts ever few ticks to move character according to how calcMovement has been changed. It will be 0 on default
        //Get what keys are being pushed and save into a 2-value vector (Basically just an array with two values).
			calcMovement = (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
   }

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

	protected override void MakeVibration () {
		//Define Ring size so that walking causes a uniform speed, while running or crawling has double the effect
		int ringSize = (int) (speed*2 - WALK_SPEED)/INVERSE_RING_SIZE;
		//Create Vibration Ring
		Vibration.Vibrator().MakeVibration(ringSize, (Vector2) transform.position + FEET_POSITION + calcMovement/2, gameObject);
	}

	//When Vibration is felt from other objects
	public override void FeelVibration (Vector2 sourcePosition) {
		Debug.Log("Touched by a Vibration!!");
	}

	public override void PickUp(GameObject item) {
		Inventory.instance.PickUp(item);
	}

	public void run()
	{
		speed = RUN_SPEED;
	}

	public void walk()
	{
		speed = WALK_SPEED;
	}
}
