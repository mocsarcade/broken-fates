using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour {

    //Initialize important variables
    public float speed;
    public float maxVel;
    protected Rigidbody2D rb2d;
	public Vector2 calcMovement;
	protected bool frozen;

	// rb2d gets its component every time this script is "enabled". Basically when the script begins.
    void OnEnable()
    {
		//Initialize RigidBody, which will move the object when it is moved
        rb2d = GetComponent<Rigidbody2D>();
    }

	void Awake()
	{
		frozen = false;
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
		//We don't want our objects to go too fast, so first we must check that the objects' velocity hasn't grown too high
		if (calcMovement.magnitude < maxVel && calcMovement.magnitude > 0.1 && frozen == false) {
			//Move the rigidbody by applying force, and the object will move too
			rb2d.AddForce (calcMovement * speed);//, ForceMode.VelocityChange);
			//Check object's method to see if a vibration should be made
			CheckVibration();
		} else if (calcMovement.magnitude >= maxVel && frozen == false) {
			//If speed is too high, just decrease it to fit the maxvel
			rb2d.AddForce (calcMovement * maxVel / calcMovement.magnitude);//, ForceMode.VelocityChange);
			//Check object's method to see if a vibration should be made
			CheckVibration();
		}
		//Add force to move our character. VelocityChange ignores mass to remove stopping latency
    }
	
	//The "virtual" is important to show this method will be overriden
	//This is the method that will be overriden in any method that inherits this class. This is how all moving objects decide their move patterns
	protected virtual void ComputeVelo () {}

	//The "virtual" is important to show this method will be overriden
	//This is how all moving objects decide their move animations
	protected virtual void UpdateAnimator (Vector2 direction) {}

	//The "virtual" is important to show this method will be overriden
	//This is how all moving objects decide when vibrations are made
	protected virtual void CheckVibration () {}

	// When a script is called or time is stopped, the player will have to freeze
	public void SetMobility(bool mobility)
	{
		frozen = mobility;
	}

}
