using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMemento : Memento {

		//Keeping Track Variables
		public bool inInventory;
		public bool curItemState;
		//This variable is set at the item's creation if the item was in the players' inventory at save-time
		//It will be used at load-time to return the object ot the correct spot in the inventory
		public int inventoryIndex;

		public void InitializeInventory(bool flag) {
			inInventory = flag;
			curItemState = flag;
		}

		public void setInInventory(bool flag, int index) {
			curItemState = flag;
			if(flag == true) {
				inventoryIndex = index;
			}
		}

		public override void Revert() {
			if(inInventory == false && curItemState == false) {
				parent.useMemento(this);
			}	else if (inInventory == false && curItemState == true) {
				//Get item out of inventory and create at position/in hand of pickedUp
		    Instantiate(Inventory.instance.GetItem(inventoryIndex).concreteObject, position, Quaternion.identity).GetComponent<Material>().PickedUp(parent.gameObject);
				Inventory.instance.Remove(inventoryIndex);
			} else if (inInventory == true && curItemState == false) {
				//Destroy memento and place back in inventory
				Inventory.instance.TransferIn(inventoryIndex, parent);
			}
		}
}
