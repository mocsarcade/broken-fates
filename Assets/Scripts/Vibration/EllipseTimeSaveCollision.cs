using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EllipseTimeSaveCollision : EllipseVibrationCollision {

	public override void OnTriggerEnter2D(Collider2D touched) {
		GameObject touchedObj = touched.gameObject;
		parent.GetComponent<QuickLife>().SaveObject(touchedObj);
	}
}
