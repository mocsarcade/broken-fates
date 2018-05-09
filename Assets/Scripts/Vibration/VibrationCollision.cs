using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationCollision : MonoBehaviour {

	private CircleCollider2D _circleCollider2D;

	//Object that made this vibration
	private GameObject parent;

	// Use this for initialization
	void Awake () {
		_circleCollider2D = GetComponent<CircleCollider2D>();
	}

	public void Initialize(GameObject _parent) {
		parent = _parent;
	}

	public void UpdateCircle(List<Vector2> _vertices) {
		_circleCollider2D.radius = Vector2.Distance(_vertices[0], _vertices[1]);
	}

	void OnTriggerEnter2D(Collider2D touched) {
		GameObject touchedObj = touched.gameObject;
		if(touchedObj != parent) {
			touchedObj.SendMessage("FeelVibration", (Vector2) transform.position, SendMessageOptions.RequireReceiver);
		}
	}
}
