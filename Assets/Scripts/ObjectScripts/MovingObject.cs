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
  public Vector2 handOffset;
  public int handSortOrder;

  protected override void Awake() {
    base.Awake();
    animator = GetComponent<Animator>();
    handOffset = new Vector2(0,0);
  }

  //Load the memento for moving objects
  protected override void LoadMemento() {
    MementoType = (GameObject) Resources.Load("MovingObjectMemento");
  }

	//Every update, the players' movement will be calculated using the ComputeVelo method of the inheriting objects
	protected override void Update() {
    //Debug.Log(animator.GetBehaviour<AnimatorSaveScript>());

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
      //rb2d.AddForce (moveDirection * speed);//, ForceMode.VelocityChange);
      shadow.Push (moveDirection * speed, ForceMode2D.Force);
  		//Check object's method to see if a vibration should be made - this has been replaced with animator events
  	} else if (moveDirection.magnitude >= maxVel) {
  		//If speed is too high, just decrease it to fit the maxvel
      shadow.Push (moveDirection * speed, ForceMode2D.Force);
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

  //Any object in the player's hand will be visible actually appearing in the player's hand.
  //This is achieved by animator events for every animation clip that tell the script to
  //update where the player's hand is.
  //@param x_percent is the x amount of pixels in PERCENT OF THE OBJECT SIZE that is traversed to reach the hand divided by 2
  //@param y_percent is the x amount of pixels in PERCENT OF THE OBJECT SIZE that is traversed to reach the hand divided by 2
  //ex: hand of player is 9 of 19 pixels in. That is ((9/19)*100)/2
  //public void SetHandPosition(float x_percent, int y_percent) {
  public void SetHandPosition(AnimationEvent animEvent) {
    Bounds rt = myRenderer.sprite.bounds;
    float x_percent = animEvent.floatParameter;
    float y_percent = animEvent.intParameter;
    Vector3 spriteSize = rt.extents;
    //Set offset
    handOffset = new Vector2(((spriteSize.x*x_percent)/100), -((spriteSize.y*y_percent)/100));
    //Set Sorting Order if string for method calls for it
    if(animEvent.stringParameter == "pushBack") {
      handSortOrder = myRenderer.sortingOrder-1;
    } else {
      handSortOrder = myRenderer.sortingOrder+1;
    }
  }

  //This method uses the handOffset variable from the object's animator to calcluate and place
  //objects this object is holding
	public override Vector3 GetHeldPosition(Vector3 oldPosition) {
    //The hand object is placed according to the object's actual transform; not the shadow
		return transform.position + (Vector3) handOffset;
	}

	public override int GetHeldSortingOrder() {
		return handSortOrder;
	}

}
