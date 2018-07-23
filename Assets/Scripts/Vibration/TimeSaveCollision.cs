using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSaveCollision : VibrationCollision {

	public List<GameObject> savedObjects = new List<GameObject>();

	public override void OnTriggerEnter2D(Collider2D touched) {
		GameObject touchedObj = touched.gameObject;
		if(touchedObj != null) {
			//Check that this object hasn't already been saved
			/*foreach(GameObject includedMem in savedObjects) {
				if(includedMem == touchedObj) {
					return;
				}
			}*/
			if(savedObjects.Contains(touchedObj)) {return;}
			//Save parent object of the touched shadow
			parent.GetComponent<QuickLife>().SaveObject(touchedObj.GetComponent<Shadow>().GetParent().gameObject);
		}

	}
}
