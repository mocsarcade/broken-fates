using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : Material {

  //Initialize important variables
  protected Animator animator;
  public float speed;
  public float maxVel;
	public Vector2 calcMovement;
	public bool frozen = false;

  protected override void Awake() {
    base.Awake();
    animator = GetComponent<Animator>();
  }

  //Load the memento for moving objects
  protected override void LoadMemento() {
    MementoType = (GameObject) Resources.Load("MovingObjectMemento");
  }

	//Every update, the players' movement will be calculated using the ComputeVelo method of the inheriting objects
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

  public void Move(Vector2 moveDirection) {
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

	//This is how dashing objects render dashing, as it interfaces with the dash mechanic that the object may use
	public virtual void AnimateDash (Vector2 direction) {}

	//The "virtual" is important to show this method will be overriden
	//This is how all moving objects decide when vibrations are made
	protected virtual void MakeVibration () {}

  // When a script is called or time is stopped, the player will have to freeze
  public void SetMobility(bool mobility)
  {
    frozen = !mobility;
  }

  //Simple method used by MovingObjectMemento to save the object's animator state
	public Animator GetAnimator () {return animator;}

  public Vector2 GetMovement() {return calcMovement;}

  //Picks up this object and returns null, telling the program the Use()
  //function cannot be done on this item while in the players' hand
  public override Item PickedUp(GameObject holder) {
    base.PickedUp(holder);
    //Freeze the object's movements
    SetMobility(false);
    return null;
  }

  //Interface for throwing so all objects can throw an object if they are holding something.
  public virtual void throwHeldObject(Vector2 tarPos) {}

}
