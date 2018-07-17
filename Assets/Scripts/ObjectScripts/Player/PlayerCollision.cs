﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

	private const float SPEAKING_DISTANCE = 0.75f;
	private const int layerMask = 2048; //This layermask is binary for 0100000000000, which means this layermask is for layer 11


	//Singleton varaible
	public Player playerInstance;
	public Collider2D playerCollider;
	public Transform playerTransform;

	void Awake()
	{
		playerTransform = gameObject.transform;
	}

	//The start method is run after the awake method. The Player's instance variable
	//is set during awake, so referencing it is done afterwards
	void Start() {
		playerInstance = Player.GetPlayer();
		playerCollider = playerInstance.gameObject.GetComponent<Collider2D>();
	}

	// Update is called once per frame
	void Update () {
    if(Input.GetButtonDown("PickUp"))
    {
			playerCollider.enabled = false;
			RaycastHit2D touched = Physics2D.CircleCast(
				(Vector2) playerTransform.position,
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
