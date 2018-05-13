using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

	public const float NUM_SHADOW_IMAGES = 14f;

	public Transform parentTransform;
	public Rigidbody2D rb2d;
	public Animator animator;
	public int size;
	public bool following = true;
	public float y_offset;

	void OnEnable() {
		rb2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	public void Initialize(Transform _parent, int _size, float _offset) {
		parentTransform = _parent;
		size = _size;
		y_offset = _offset;
		animator.SetFloat("Size", size/NUM_SHADOW_IMAGES);
	}

	void Update () {
		if(following)
			transform.position = parentTransform.position + Vector3.down * y_offset;
	}

	public void UpdateSize (int z) {
		int dist = z/5;
		if(dist>size)
			dist=size;
		else if(dist<0)
			dist=0;
		animator.SetFloat("Size", ((float) size-dist)/size);
	}

	public void Push (Vector2 force) {
		rb2d.AddForce(force, ForceMode2D.Impulse);
	}

	public void setPosition (Vector2 newPosition) {
		//rb2d.MovePosition(newPosition + offset);
		transform.position = (Vector3) (newPosition + Vector2.down * y_offset);
	}

	public Vector2 getPosition () {
		return (Vector2) transform.position - Vector2.down * y_offset;
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
