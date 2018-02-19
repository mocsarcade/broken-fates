using UnityEngine;

// CreateAssetMenu allows you to construct a new item as you would a new C# script. Right click in assets, check under Inventory, then Item. 
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {

    // Every item needs to have an id that can be referenced in the future. So, calls to items in scripts should be done by ID.
    public int ID;
    public string description;
    new public string name;
    public Sprite icon = null;
    public ItemType Type;
    
    public enum ItemType
    {
        Consumable,
        Equipment,
        Key
    };
}
