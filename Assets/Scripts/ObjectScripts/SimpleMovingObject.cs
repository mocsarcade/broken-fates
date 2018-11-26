using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovingObject : MonoBehaviour {

	 //Initialize important variables
	public float speed;
	public float maxVel;
	public Vector2 calcMovement;
	protected bool frozen = false;

  public Rigidbody2D rb2d;

	//public virtual void Awake() {
	void Awake() {
    rb2d = GetComponent<Rigidbody2D>();
	}

	//Every update, the object's movement will be calculated using the ComputeVelo method of the inheriting objects
	void Update() {
		calcMovement = Vector2.zero;
		//Get movement direction and set animator vars
		ComputeVelo();
		UpdateAnimator(calcMovement);
	}

	//Every fixed update, the player will move. This will keep the players' movements uniform regardless of computer lag
  void FixedUpdate () {
    if(frozen == false) {
    	Move(calcMovement);
    }
  }

  public virtual void Move(Vector2 moveDirection) {
	//We don't want our objects to go too fast, so first we must check that the objects' velocity hasn't grown too high
  	if (moveDirection.magnitude < maxVel && moveDirection.magnitude > 0.1) {
  		//Move the rigidbody by applying force, and the object will move too
  		rb2d.AddForce (moveDirection * speed);//, ForceMode.VelocityChange);
  		//Check object's method to see if a vibration should be made - this has been replaced with animator events
  	} else if (moveDirection.magnitude >= maxVel) {
  		//If speed is too high, just decrease it to fit the maxvel
  		rb2d.AddForce (moveDirection * maxVel / moveDirection.magnitude);//, ForceMode.VelocityChange);
  		//Check object's method to see if a vibration should be made - this has been replaced with animator events
  	}
	   //Add force to move our character. VelocityChange ignores mass to remove stopping latency
  }

		//The "virtual" is important to show this method will be overriden
		//This is the method that will be overriden in any method that inherits this class. This is how all moving objects decide their move patterns
		protected virtual void ComputeVelo () {}

	//The "virtual" is important to show this method will be overriden
	//This is how all moving objects decide their move animations
	protected virtual void UpdateAnimator (Vector2 direction) {}
}
