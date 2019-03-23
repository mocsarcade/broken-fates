using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VibrationCollision : MonoBehaviour {

	//private EllipseCollider2D _ellipseCollider2D;
  EdgeCollider2D edgeCollider;

	//Object that made this vibration
	protected GameObject parent;

	// Use this for initialization
	void Awake () {
		//_ellipseCollider2D = GetComponent<EllipseCollider2D>();
    edgeCollider = GetComponent<EdgeCollider2D>();
	}

	public void Initialize(GameObject _parent) {
		parent = _parent;
	}

	public void UpdateEllipse(Vector3[] _vertices) {
		edgeCollider.points = ConvertArray(_vertices);
	}

	public virtual void OnTriggerEnter2D(Collider2D touched) {
		GameObject touchedObj = touched.gameObject;
		if(touchedObj != parent) {
			Shadow touchedShadow = touchedObj.GetComponent<Shadow>();
			if(touchedShadow != null) {
				touchedShadow.FeelVibration(((Vector2) transform.position));
			}
		}
		Trap touchedTrap = touchedObj.GetComponent<Trap>();
		if(touchedTrap != null) {
			touchedTrap.FeelVibration(((Vector2) transform.position));
		}
	}

	public Vector2[] ConvertArray(Vector3[] v3){
		Vector2 [] v2 = new Vector2[v3.Length];
		for(int i = 0; i <  v3.Length; i++){
				Vector3 tempV3 = v3[i];
				v2[i] = new Vector2(tempV3.x, tempV3.y);
		}
		return v2;
	}

}
