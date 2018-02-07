using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour {

    // It occured to me that we needed to make a seperate class, InventoryItem, so that we could count quantity in the player's inventory. - JRT
    public Item Details { get; set; }
    public int Quantity { get; set; }

    public InventoryItem(Item details, int quantity)
    {
        Details = details;
        Quantity = quantity;
    }
}
