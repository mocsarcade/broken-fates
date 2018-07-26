using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject target;

	private const float SHAKE_RATIO = 0.075f;
	private float shakeOffset;

	private Vector3 offset;

	// Use this for initialization
	void Start () {
		//This commented part hard-coded the player as the followed object. Leaving target open is more flexible
		//target = Player.GetPlayer().gameObject;
		offset = transform.position - target.transform.position;
	}

	// Update is called once per frame
	void LateUpdate () {
		if(target) {
			transform.position = target.transform.position + offset + new Vector3(shakeOffset,0,0);
		}
	}

	//Sets offset of camera
	public void Shake(int amount) {
		shakeOffset = amount*SHAKE_RATIO;
	}

	/*
	void fixedUpdate() {

	}*/

}
