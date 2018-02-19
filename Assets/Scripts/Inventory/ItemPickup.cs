using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach ItemPickup component to an Item
public class ItemPickup : MonoBehaviour {

    public Item item;
    public float pickupRange = 1;

    // We will need to see where the item and player are in relation to each other
    // thisItem will take the transform value of the item it is attached to
    private Transform thisItem;
    private Transform player;

    private void Awake()
    {
        thisItem = this.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1) && (this.Distance() <= pickupRange))
        {
            PickUp();
        }
    }

    public void PickUp()
    {
        // Add returns true if it an item can be picked up.
        bool wasPickedUp = Inventory.instance.Add(item);
        if (wasPickedUp)
        {
            Debug.Log("Deleting object.");
            DestroyObject(gameObject, 0);
        }
    }

    public float Distance()
    {      
        return Vector3.Distance(thisItem.position, player.position);
    }

}
