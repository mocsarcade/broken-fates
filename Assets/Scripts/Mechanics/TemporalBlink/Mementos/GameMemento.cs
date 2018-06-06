using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMemento : Memento {

		public int handIndex;

		// Use this for initialization
		public void InitializeGame (GameManager _parent) {
			handIndex = Inventory.instance.getInventoryIndex();
		}

		public override void Revert() {
			//Start a coroutine to set players' handIndex to what it's supposed to be
			GameManager.instance.RevertHand(handIndex);
		}
}
