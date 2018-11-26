using UnityEngine;

// CreateAssetMenu allows you to construct a new item as you would a new C# script. Right click in assets, check under Inventory, then Item.
[CreateAssetMenu(fileName = "New MementoData", menuName = "Data/MementoData")]
public class MementoData : Data {

	//Item Memento Variables
	public bool curItemState;
	public int inventoryIndex;

}
