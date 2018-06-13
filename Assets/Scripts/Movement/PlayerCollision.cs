using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

	private const float SPEAKING_DISTANCE = 0.75f;

	//Singleton varaible
	public Player playerInstance;
	public BoxCollider2D playerCollider;
	public Transform playerTransform;

	void Awake()
	{
		playerCollider = GetComponent<BoxCollider2D>();
		playerTransform = gameObject.transform;
	}

	//The start method is run after the awake method. The Player's instance variable
	//is set during awake, so referencing it is done afterwards
	void Start() {
		playerInstance = Player.getPlayer();
	}

	// Update is called once per frame
	void Update () {
    if(Input.GetButtonDown("PickUp"))
    {
			int layerMask = 10240; //This layermask is binary for 0010100000000000, which means this layermask includes layers 11 and 13
			playerCollider.enabled = false;
			RaycastHit2D touched = Physics2D.CircleCast((Vector2) playerTransform.position,
				SPEAKING_DISTANCE,
				playerInstance.GetDirection(),
				SPEAKING_DISTANCE,
				layerMask);
				playerCollider.enabled = true;
			//Pick up the object
			if(touched.collider != null) {
				playerInstance.PickUp(touched.collider.gameObject);
			}
    }
	}
}
