using UnityEngine;

// CreateAssetMenu allows you to construct a new item as you would a new C# script. Right click in assets, check under Inventory, then Item.
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {

	public string description;
	new public string name;
	public Sprite icon = null;
	public ItemType Type;
	//This variable will hold the prefab for the concrete version of the object
	//This will be useful when the object is being thrown, which will need the material object to be instantiated
	public GameObject concreteObject;

	public enum ItemType
	{
			Object,
			Consumable,
			Equipment,
			Key
	};

	//Use() method will go here and will call Use() method on effect

}
