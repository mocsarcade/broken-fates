//NOTE FROM AN EDITOR: When the time comes to add running, just change the "speed" varaible from MovingObject. The default speed is 150, and changing that will change the players' speed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MovingObject
{

	private Animator animator;
	public const int WALK_SPEED = 50;
	public const int RUN_SPEED = 150;
	public const int INVERSE_RING_SIZE = 2;
	public const int VIBRATION_DELAY = 25;

	private int count;

    // animator gets its component every time this script is "enabled". Basically when the script begins.
    void Awake()
    {
        animator = GetComponent<Animator>();
		count = 0;
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

	protected override void CheckVibration () {
		count++;
		if (count > VIBRATION_DELAY) {
			//Make Vibration at my position and call Initialization method to set age of ring
			Instantiate(Vibration, transform.position, Quaternion.identity).GetComponent<DrawCircle>().Initialize ((int) speed/INVERSE_RING_SIZE);
			count = 0;
		}
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
