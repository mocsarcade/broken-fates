using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteItem : Material {

	//NOTE: ConcreteItem is meant to be found in the dungeon and then turn invisible once picked up.
	//At all times, a ConcreteItem object is a child of the player as an invisible version of the item
	//in the player's hand. This object is referenced for throwing
	public Item inventoryCounterpart;
	public ItemScript useScript;

	//public SpriteRenderer myRenderer;
	//public SpriteRenderer shadowRenderer;

	//Memento Variables
	public bool curItemState;
	//This variable is set at the item's creation if the item was in the players' inventory at save-time
	//It will be used at load-time to return the object ot the correct spot in the inventory
	public int inventoryIndex;

	// animator gets its component every time this script is "enabled". Basically when the script begins.
	protected override void Awake()
	{
		base.Awake();
		useScript = GetComponent<ItemScript>();
	  //myRenderer = gameObject.GetComponent<SpriteRenderer>();
	  //shadowRenderer = shadow.GetComponent<SpriteRenderer>();
	}

	protected override void LoadMemento() {
		MementoType = (GameObject) Resources.Load("ItemMemento");
	}

	//Create method for when TimeVibration hits the object
	public override Memento CreateMemento() {
		//Keep track of item's memento so that should the item be placed in the inventory, it can be retrieved
		if(curMemento == null) {
			curMemento = base.CreateMemento();
			return curMemento;
		} else {
			curMemento.Initialize(this);
      return curMemento;
		}
	}

	//Create method for if this item is in the inventory when it was saved.
	//This method is used to add mementos to the inventory, which needs ItemMementos
	//CreateMemento and CreateInventoryMemento are built to combine for the heldItem
	public ItemMemento CreateInventoryMemento() {
		if(curMemento == null) {
			mementoData = Object.Instantiate((MementoData) GameManager.instance.getDataReference(GameManager.DataType.t_MementoData));
			curMemento = Instantiate(MementoType).GetComponent<Memento>();
			mementoData.curItemState = true;
			curMemento.Initialize(this);
			return curMemento as ItemMemento;
		} else {
			mementoData = Object.Instantiate((MementoData) GameManager.instance.getDataReference(GameManager.DataType.t_MementoData));
			mementoData.curItemState = true;
			curMemento.Initialize(this);
			return curMemento as ItemMemento;
		}
	}

	public void setInInventory(bool flag, int index) {
		mementoData.curItemState = flag;
		if(flag == true) {
			mementoData.inventoryIndex = index;
		}
	}

	/*
	public ItemMemento GetMemento() {
		return curMemento as ItemMemento;
	}*/
	/*
	private void setVisibility(bool state) {
		myRenderer.enabled = state;
		shadowRenderer.enabled = state;
	}*/

  //Picks up this object and returns null, telling the program the Use()
  //function cannot be done on this item while in the players' hand
  public override Item PickedUp(GameObject holder) {
		//Pick up
		base.PickedUp(holder);
    //Return this object's inventory counterpart so it can be rendered and added to the inventory
    return inventoryCounterpart;
  }

  public Item GetItem() {
		return inventoryCounterpart;
	}

	// Use this item
	public override void Use () {
		if(useScript != null) {
			//Do its effect
			bool destructionFlag = useScript.Use();
			//If this item is destroyed upon use:
			if(destructionFlag == true) {
				//Remove this object from the player's inventory
				Inventory.instance.destroyHandObject();
			}
		} else {
			Debug.Log("This object has no useScript! Why are you trying to use it?");
		}
	}
}
