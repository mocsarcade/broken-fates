using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : SimpleMovingObject {

	public float strength;

	public float getStrength() {
		return strength;
	}

	//DUMMY METHOD. THIS WILL BE CHANGED WHEN GHOST BECOMES MOVING ENEMY
  protected override void ComputeVelo () {
    //Don't do anything (this will change when ghosts become dynamic, moving enemies)
		calcMovement = new Vector2(0, 0);
	}
}
