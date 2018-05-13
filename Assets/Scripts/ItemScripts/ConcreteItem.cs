using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteItem : Material {


	//NOTE: ConcreteItem is meant to be found in the dungeon and then turn invisible once picked up.
	//At all times, a ConcreteItem object is a child of the player as an invisible version of the item
	//in the player's hand. This object is referenced for throwing
	public Item inventoryCounterpart;

	public SpriteRenderer myRenderer;
	public SpriteRenderer shadowRenderer;

	// animator gets its component every time this script is "enabled". Basically when the script begins.
	protected override void Awake()
	{
		base.Awake();
	  myRenderer = gameObject.GetComponent<SpriteRenderer>();
	  shadowRenderer = shadow.GetComponent<SpriteRenderer>();
	}

	//If this object is an invisible object to follow the player, this method is called to make invisible
  public void Initialize(GameObject player) {
    //Make this object invisible as it has now been picked up
		setVisibility(false);
		//Attach to player
		Transform playerPosition = Player.getPlayer().gameObject.transform;
		transform.position = playerPosition.position;
		transform.parent = playerPosition;
  }

	private void setVisibility(bool state) {
		myRenderer.enabled = state;
		shadowRenderer.enabled = state;
	}

	public override IEnumerator Throw(Vector2 start, Vector2 target, float strength) {
		//Make this object visible and in proper position
		setVisibility(true);
		//Detatch from player
		transform.parent = null;
		//Activate appropriate player animation

		//Throw
		StartCoroutine(base.Throw(start, target, strength));
		yield return null;
	}

  //Picks up this object and returns null, telling the program the Use()
  //function cannot be done on this item while in the players' hand
  public override Item pickUp(GameObject holder) {
    //Make this object invisible as it has now been picked up
		setVisibility(false);
		//This object may be used again if it is thrown, so make this object follow the one who picked it up rather than destroy it
		transform.parent = holder.transform;
    //Return this object's inventory counterpart so it can be rendered and added to the inventory
    return inventoryCounterpart;
  }

	// Use this item
	public void Use () {
		//Do its effect

		//If this item is destroyed upon use

			//Send back a message to the Inventory to refill the players' hand

			//Destroy the item
	}
}
