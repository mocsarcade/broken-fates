using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSummon : Mechanics {

    //This can be changed in the inspector to change how much stamina is taken by summoning a spell
	public int MAGIC_COST = 100;

	public GameObject rock;
	public GameObject target;
	public Vector2 tarPos;
	public GameObject playerObject;

	private PlayerMovement playerMoveScript;
	private GameObject magicTar;
	
	// Use this for initialization
	void Start () {
		tarPos = transform.position;
		playerMoveScript = playerObject.GetComponent<PlayerMovement>();
	}
	
	// Update is called once per frame
	void Update () {
		
		//Below is the script that creates target and destroys target at the push of a button. Movement of the target is in the target's own movement script. Later this pseudocode will be placed in its own method inside this class that is called at update, rather than having so much stuff in the update. The calling will be done in a for loop, that calls the method once for each potential key the mechanic could be used for... or just check whether the mechanic is chosen for a key before calling the method
		
		if(Input.GetKeyDown(KeyCode.F)) {
			//Find position of camera which will be the default placing of the magic targetter
			tarPos = transform.position;
			//Variable magicTar is used for when magicTar has to be destroyed when the activation key (ex: F) is released
			magicTar = Instantiate(target, tarPos, Quaternion.identity);
			//Freeze player so he won't move while target is moving
			playerMoveScript.Halt();
		}
		if(Input.GetKeyUp(KeyCode.F)) {
			if((Vector2) magicTar.transform.position != tarPos) {
				//Find target's position so rock can be created there
				tarPos = magicTar.transform.position;
				//Create Rock at position
				Instantiate(rock, tarPos, Quaternion.identity);
				//Drain stamina for using spell
				DrainStam(MAGIC_COST);
				//Destroy target
				Destroy(magicTar);
			}
			else {
				//Destroy target and do nothing for having target over player
				Destroy(magicTar);
			}
			//Unfreeze player when target has made the rock
			playerMoveScript.Resume();
		}
	}
	
}
