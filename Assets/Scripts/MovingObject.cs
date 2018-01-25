using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour {

    //Initialize important variables
    public float speed;
    public float maxVel;
    protected Rigidbody2D rb2d;
	public Vector2 calcMovement;
	
	// rb2d gets its component every time this script is "enabled". Basically when the script begins.
    void OnEnable()
    {
		//Initialize RigidBody, which will move the object when it is moved
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	//Every update, the players' movement will be calculated using the ComputeVelo method of the inheriting objects
	void Update() {
		calcMovement = Vector2.zero;
		ComputeVelo();
	}
	
	//Every fixed update, the player will move. This will keep the players' movements uniform regardless of computer lag
    void FixedUpdate () {
		//We don't want our objects to go too fast, so first we must check that the objects' velocity hasn't grown too high
		if(Mathf.Abs(rb2d.velocity.x) < maxVel && Mathf.Abs(rb2d.velocity.y) < maxVel) {
			//Move the rigidbody by applying force, and the object will move too
            rb2d.AddForce(calcMovement * speed);
        }
    }
	
	//The "virtual" is important to show this method will be overriden
	//This is the method that will be overriden by findVelo in any method that inherits this class. This is how all moving objects decide their move patterns
	protected virtual void ComputeVelo () {
		
	}

}
