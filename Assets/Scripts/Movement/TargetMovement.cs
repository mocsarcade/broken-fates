using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : SimpleMovingObject {

    protected override void ComputeVelo () {
        //Movement method of MovingObject acts ever few ticks to move character according to how calcMovement has been changed. It will be 0 on default
        //Get what keys are being pushed and save into a 2-value vector (Basically just an array with two values).
				calcMovement = (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
	}

}
