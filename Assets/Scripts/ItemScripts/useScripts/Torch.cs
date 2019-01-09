using UnityEngine;

public class Torch : ItemScript {

	public override bool Use () {
		//Find all torches
		GameObject[] torches = GameObject.FindGameObjectsWithTag("Lightable");
		foreach(GameObject lightable in torches) {
				float distance = Vector3.Distance(lightable.transform.position, transform.position);
				if(distance < GlobalRegistry.PLAYER_REACH()*3f) {
						// Light up that object
						LightableTorch lightScript = lightable.GetComponent<LightableTorch>();
						if(lightScript) {
							lightScript.Light();
						}
				}
		}
		//Torches are not destroyed upon use
		return false;
	}
}
