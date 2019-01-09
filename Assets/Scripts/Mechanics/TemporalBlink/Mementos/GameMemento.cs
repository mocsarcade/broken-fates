using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMemento : Memento {

		private int handIndex;
		private int StaminaCap;

		// Use this for initialization
		public void InitializeGame (GameManager _parent) {
			handIndex = Inventory.instance.GetInventoryIndex();
			StaminaCap = GameManager.instance.GetCap();
		}

		public override void Revert() {
			//Start a coroutine to set players' handIndex to what it's supposed to be
			GameManager.instance.RevertHand(handIndex);
			GameManager.instance.setCap(StaminaCap);
		}
}
