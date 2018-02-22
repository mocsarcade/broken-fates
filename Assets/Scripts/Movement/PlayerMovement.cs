//NOTE FROM AN EDITOR: When the time comes to add running, just change the "speed" varaible from MovingObject. The default speed is 150, and changing that will change the players' speed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MovingObject
{

    private Animator animator;

    // animator gets its component every time this script is "enabled". Basically when the script begins.
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelo()
    {
        //Movement method of MovingObject acts ever few ticks to move character according to how calcMovement has been changed. It will be 0 on default
        //Get what keys are being pushed and save into a 2-value vector (Basically just an array with two values).
			calcMovement = (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

            //Set Move and MoveX which tell animator what direction our player should be facing (unless player is idle, in which case direction will be saved for animator
			if(calcMovement.magnitude > 0.1 && frozen == false)
				{
					if(animator.GetBool("Moving") == false)
					{
						animator.SetBool("Moving", true);
					}
					animator.SetFloat("MoveY", calcMovement.y);
					animator.SetFloat("MoveX", calcMovement.x);
				}
				else
				{
					if(animator.GetBool("Moving") == true)
					{
					animator.SetBool("Moving", false);
					}
				}
    }
}