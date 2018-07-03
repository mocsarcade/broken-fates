using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMemento : Memento {

		//Keeping Track Variables
		public bool inInventory;
		//This variable is set at the item's creation if the item was in the players' inventory at save-time
		//It will be used at load-time to return the object ot the correct spot in the inventory

		public override void Initialize (Material _parent) {
			base.Initialize(_parent);
			inInventory = myData.curItemState;
		}

		public void InitializeInventory (int index) {
			inInventory = true;
			myData = Object.Instantiate((MementoData) GameManager.instance.getDataReference(GameManager.DataType.t_MementoData));
			myData.curItemState = inInventory;
			myData.inventoryIndex = index;
		}

		public override void Revert() {
			//Get whether the item is currently in the player's inventory
			bool curItemState = myData.curItemState;
			if(inInventory == false && curItemState == false) {
					//If item was picked up and thrown somewhere else, place it back (or recreate it if it was destroyed)
					base.Revert();
				} else if (inInventory == false && curItemState == true) {
				//Get item out of inventory and create at position/in hand of pickedUp
		    Instantiate(Inventory.instance.GetItem(myData.inventoryIndex).concreteObject, position, Quaternion.identity).GetComponent<Material>().PickedUp(parent.gameObject);
				Inventory.instance.Remove(myData.inventoryIndex);
			} else if (inInventory == true && curItemState == false) {
				//Destroy memento and place back in inventory
				ConcreteItem itemParent = parent as ConcreteItem;
				if(itemParent)
					itemParent.EmptyMemento();
				Inventory.instance.TransferIn(myData.inventoryIndex, parent);
			}
		}

		public void setInInventory(bool flag, int index) {
			myData.curItemState = flag;
			if(flag == true) {
				myData.inventoryIndex = index;
			}
		}
}
