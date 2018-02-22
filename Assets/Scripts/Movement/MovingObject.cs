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
		ComputeVelo();
	}
	
	//Every fixed update, the player will move. This will keep the players' movements uniform regardless of computer lag
    void FixedUpdate () {
		//We don't want our objects to go too fast, so first we must check that the objects' velocity hasn't grown too high
		if(calcMovement.magnitude< maxVel && calcMovement.magnitude > 0.1 && frozen == false) {
			//Move the rigidbody by applying force, and the object will move too
			rb2d.AddForce(calcMovement * speed);//, ForceMode.VelocityChange);
			//Add force to move our character. VelocityChange ignores mass to remove stopping latency
		}
    }
	
	//The "virtual" is important to show this method will be overriden
	//This is the method that will be overriden by findVelo in any method that inherits this class. This is how all moving objects decide their move patterns
	protected virtual void ComputeVelo () {
		
	}

	// When a script is called or time is stopped, the player will have to freeze
	public void SetMobility(bool mobility)
	{
		frozen = mobility;
	}

}
