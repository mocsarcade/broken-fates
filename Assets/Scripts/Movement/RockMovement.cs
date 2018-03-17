using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMovement : MonoBehaviour {
	
	//This will be changed as the uniform height of a rock as it is summoned will change
	public int DEFAULT_Z;
	
	//z is how "high" a rock is. A rock is summoned at a uniform height and then falls to the ground. Z is that height as it changes
	private int z;
	
    protected Rigidbody2D rb2d;
	public Vector2 realPosition;
	public Vector2 curPosition;
	public Vector2 speed;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
		//save real position for when z changes and when making shadow
		realPosition = transform.position;
		//set position to accomodate z
		  //Create temp variable to hold what visible position will be
		  curPosition = realPosition;
		  curPosition.y -= z;
		  //Change position
		  transform.position = curPosition;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Set z to decrease so it will stop when it's about to go negative
		if(transform.position.y < realPosition.y + speed.y) {
			rb2d.AddForce(speed);
		}
	}
}
