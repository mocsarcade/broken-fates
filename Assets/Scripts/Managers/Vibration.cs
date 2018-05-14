using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour {

	protected static Vibration _instance = null;
	public GameObject VibrationPrefab;

	// Make Singleton by using a static instance of Vibration
	public static Vibration Vibrator () {
		if (_instance == null) {
			GameObject VibrationManager = (GameObject) Instantiate(Resources.Load("VibrateManager"));
			_instance = VibrationManager.GetComponent<Vibration>();
		}
		return _instance;
	}

	public void MakeVibration(int ringSize, Vector2 myPosition, GameObject parent) {
		//Create Vibration Ring
		Instantiate(VibrationPrefab, myPosition, Quaternion.identity).GetComponent<DrawShape>().Initialize (ringSize, parent);
	}

}
