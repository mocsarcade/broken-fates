using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour {

	protected static Vibration _instance = null;
	public GameObject VibrationPrefab;
	public GameObject TimeVibrationPrefab;

	private List<GameObject> VibrationList = new List<GameObject>();

	// Make Singleton by using a static instance of Vibration
	public static Vibration Vibrator () {
		if (_instance == null) {
			GameObject VibrationManager = (GameObject) Instantiate(Resources.Load("VibrateManager"));
			_instance = VibrationManager.GetComponent<Vibration>();
		}
		return _instance;
	}

	//Overloaded method for making vibrations
	//This method takes a material script as the "parent"
	public void MakeVibration(int ringSize, Vector2 myPosition, Material _parent, Collider2D onWall=null) {
		//Create Vibration Ring
		GameObject _parentShadow = _parent.GetShadow().gameObject;
		//Instantiate(VibrationPrefab, myPosition, Quaternion.identity).GetComponent<DrawShape>().Initialize (ringSize, parent);
		GameObject newVibration = Instantiate(VibrationPrefab, myPosition, Quaternion.identity);
		//Add Vibration to List of Vibrations in case EraseVibrations is called
		AddToList(newVibration);
		newVibration.GetComponent<DrawVibration>().Initialize (ringSize, _parentShadow, onWall);
	}
	//This method takes an object as the "parent" (used for objects that are not materials [a.k.a, don't have shadows]
	public void MakeVibration(int ringSize, Vector2 myPosition, GameObject _parentShadowObj, Collider2D onWall=null) {
		//Create Vibration Ring
		//Instantiate(VibrationPrefab, myPosition, Quaternion.identity).GetComponent<DrawShape>().Initialize (ringSize, parent);
		GameObject newVibration = Instantiate(VibrationPrefab, myPosition, Quaternion.identity);
		//Add Vibration to List of Vibrations in case EraseVibrations is called
		AddToList(newVibration);
		newVibration.GetComponent<DrawVibration>().Initialize (ringSize, _parentShadowObj, onWall);
	}

	public void MakeTimeVibration(int ringSize, Vector2 myPosition, GameObject _parent) {
		//Create Vibration Ring
		//Instantiate(TimeVibrationPrefab, myPosition, Quaternion.identity).GetComponent<DrawShape>().Initialize (ringSize, parent);
		Instantiate(TimeVibrationPrefab, myPosition, Quaternion.identity).GetComponent<DrawVibration>().Initialize (ringSize, _parent);
	}

	private void AddToList(GameObject newVibration) {
		VibrationList.Add(newVibration);
	}

	public void RemoveVibration(GameObject destroyedVibration) {
		VibrationList.Remove(destroyedVibration);
		Destroy(destroyedVibration);
	}

	public void EraseVibrations() {
		while(VibrationList.Count > 0) {
			GameObject destroyedVibration = VibrationList[0];
			VibrationList.Remove(destroyedVibration);
			Destroy(destroyedVibration);
		}
	}

}
