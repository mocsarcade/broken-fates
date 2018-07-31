using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightableTorch : MonoBehaviour {

	public GameObject _torchLight;
	protected bool lightOn;
	protected GameObject currentLight;

	public void Light() {
		if(!lightOn) {
			currentLight = Instantiate(_torchLight, transform);
			lightOn = true;
		}
	}


}
