using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : SimpleMovingObject {

  public Vector2 origin;
  public float strength;
  public int weight;
  public const int DISTANCE_LIMIT_RATIO = 5;

  public void Initialize(Vector2 _origin, float _strength, int _weight) {
    origin = _origin;
    strength = _strength;
    weight = _weight;
  }

  protected override void ComputeVelo () {
    //Movement method of MovingObject acts ever few ticks to move character according to how calcMovement has been changed. It will be 0 on default
    //Get what keys are being pushed and save into a 2-value vector (Basically just an array with two values).
		calcMovement = (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
	}

  public override void FixedUpdate() {
    base.FixedUpdate();
    if(Vector2.Distance(transform.position, origin) > (strength/weight) * DISTANCE_LIMIT_RATIO) {
      rb2d.AddForce( ((origin - (Vector2) transform.position) * calcMovement.magnitude * speed) * (Vector2.Distance(transform.position, origin) - ((strength/weight) * DISTANCE_LIMIT_RATIO)) );// * (Vector2.Distance(transform.position, origin) - (strength/weight)*DISTANCE_LIMIT_RATIO));
    }
  }
}
