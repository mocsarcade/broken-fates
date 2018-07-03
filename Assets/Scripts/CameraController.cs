using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject player;

	private const float SHAKE_RATIO = 0.075f;
	private float shakeOffset;

	private Vector3 offset;

	// Use this for initialization
	void Start () {
		player = Player.getPlayer().gameObject;
		offset = transform.position - player.transform.position;
	}

	// Update is called once per frame
	void LateUpdate () {
		transform.position = player.transform.position + offset + new Vector3(shakeOffset,0,0);
	}

	//Sets offset of camera
	public void Shake(int amount) {
		shakeOffset = amount*SHAKE_RATIO;
	}

	/*
	void fixedUpdate() {

	}*/

}
