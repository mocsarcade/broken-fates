using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

	public Transform parentTransform;
	public Rigidbody2D rb2d;
	public int size;
	public bool following = true;
	public Vector2 offset;

	void Start() {
		rb2d = GetComponent<Rigidbody2D>();
	}

	public void Initialize(Transform _parent, int _size, Vector2 _offset) {
		parentTransform = _parent;
		size = _size;
		offset = _offset;
	}

	void Update () {
		if(following)
			transform.position = parentTransform.position + (Vector3) offset;
	}

	public void UpdateSize (int z) {
		//Write code later
		//Debug.Log("Shadow size updating");
	}

	public void Push (Vector2 force) {
		rb2d.AddForce(force, ForceMode2D.Impulse);
	}

	public void setPosition (Vector2 newPosition) {
		rb2d.MovePosition(newPosition + offset);
	}

	public Vector2 getPosition () {
		return (Vector2) transform.position;
	}

	// Detach shadow from its parent so that they move independently
	public void Detach () {
		following = false;
	}

	// Reattach Shadow so shadow will stay at the feet of the parent
	public void Attach () {
		following = true;
	}
}
