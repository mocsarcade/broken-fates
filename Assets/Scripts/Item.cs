using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    // You should be able to return ID by: nameOfItem.ID <-- You could also set the value by doing --> nameOfItem.ID = 42;
    public int ID { get; set; }
    public string itemName { get; set; }
    public string itemNamePlural { get; set; }

    public Item(int givenID, string aItemName, string aItemNamePlural)
    {
        ID = givenID;
        itemName = aItemName;
        itemNamePlural = aItemNamePlural;
    }
}
