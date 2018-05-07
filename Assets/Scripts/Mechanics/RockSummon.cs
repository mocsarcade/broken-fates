using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSummon : PlayerMechanics {

    //This can be changed in the inspector to change how much stamina is taken by summoning a spell
	public int STAMINA_COST = 100;

	public GameObject rock;
	public GameObject target;
	public Vector2 tarPos;
	public GameObject playerObject;

	private PlayerMovement playerMoveScript;
	private GameObject magicTar;

	// Use this for initialization
	void Start () {
		tarPos = transform.position;
		playerObject =GameObject.FindGameObjectWithTag("Player");
		playerMoveScript = playerObject.GetComponent<PlayerMovement>();
	}

	public override bool Activate() {
		Debug.Log("ACTIVATED!");
		tarPos = transform.position;
		//Variable magicTar is used for when magicTar has to be destroyed when the activation key (ex: F) is released
		magicTar = Instantiate(target, tarPos, Quaternion.identity);
		//Freeze player so he won't move while target is moving
		playerMoveScript.SetMobility(false);

		return true;
	}

	// Update is called once per frame
	// FUTURE UPDATES: MAKE STAMINA COST DEPENDANT ON DISTANCE FROM PLAYER
	// FUTURE UPDATES: IF STATUS RETURNS FALSE, CUT STAMINA COST IN HALF AND THROW STONE HALF THE DISTANCE REQUIRED
	public override void Release () {
			if((Vector2) magicTar.transform.position != tarPos) {
				//Drain stamina for using spell. Status is the status variable for whether player had enough stamina for activation
				bool status = GameManager.instance.DrainStamina(STAMINA_COST);
				if(status == true) {
					//Find target's position so rock can be created there
					tarPos = magicTar.transform.position;
					//Create Rock at position
					Instantiate(rock, tarPos, Quaternion.identity);
					}
			}
			//Spell cleanup
			Destroy(magicTar);
			playerMoveScript.SetMobility(true);
	}

}
