using UnityEngine;

// CreateAssetMenu allows you to construct a new item as you would a new C# script. Right click in assets, check under Inventory, then Item.
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {

	public int ID;
	public string description;
	new public string name;
	public Sprite icon = null;
	public ItemType Type;

	//This will hold the Use() method for every item
	public ItemScript effect;
	//Prefab for if item should be thrown
	public GameObject itemObject;

	public enum ItemType
	{
			Consumable,
			Equipment,
			Key
	};

	//Use() method will go here and will call Use() method on effect

}
