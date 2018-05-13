using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EllipseVibrationCollision : MonoBehaviour {

	private EllipseCollider2D _ellipseCollider2D;

	//Object that made this vibration
	private GameObject parent;

	// Use this for initialization
	void Awake () {
		_ellipseCollider2D = GetComponent<EllipseCollider2D>();
	}

	public void Initialize(GameObject _parent) {
		parent = _parent;
	}

	public void UpdateEllipse(List<Vector2> _vertices) {
		float radius = Vector2.Distance(_vertices[0], _vertices[1]);
		_ellipseCollider2D.radiusX = radius;
		_ellipseCollider2D.radiusY = 1.5f*radius;
	}

	void OnTriggerEnter2D(Collider2D touched) {
		GameObject touchedObj = touched.gameObject;
		if(touchedObj != parent) {
			touchedObj.SendMessage("FeelVibration", (Vector2) transform.position, SendMessageOptions.RequireReceiver);
		}
	}
}
