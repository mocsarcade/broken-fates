using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject {

    private Animator animator;
    private bool frozen;

    // Awake starts when the script begins
    void Awake()
    {
        animator = GetComponent<Animator>();
        frozen = false;
    }

	void Update () {
		// Currently empty
	}

    //*******************Player Inventory*********************//
    // For the player inventory, we're going to create a list of items. - JRT
    


    //***************Player Movement**********************//
    protected override void ComputeVelo()
    {
        //Movement method of MovingObject acts ever few ticks to move character according to how calcMovement has been changed. It will be 0 on default
        //Get what keys are being pushed and save into a 2-value vector (Basically just an array with two values).
        if (frozen == false)
        {
            calcMovement.x = Input.GetAxis("Horizontal");
            calcMovement.y = Input.GetAxis("Vertical");

            //Set Move and MoveX which tell animator what direction our player should be facing
            animator.SetFloat("Move", -calcMovement.y);
            animator.SetFloat("MoveX", calcMovement.x);
        }
        else
        {
            //if the player is using magic, or is frozen, his movement drops to 0
            calcMovement = Vector2.zero;

            //Animator drops to 0 and player is standing still when player is using magic, or is frozen.
            animator.SetFloat("Move", 0);
            animator.SetFloat("MoveX", 0);

        }
    }

    // animator gets its component every time this script is "enabled". Basically when the script begins.
    public void Halt()
    {
        frozen = true;
    }

    // animator gets its component every time this script is "enabled". Basically when the script begins.
    public void Resume()
    {
        frozen = false;
    }
}
