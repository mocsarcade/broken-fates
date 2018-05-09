using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    #region Singleton
    // We're going to create a singleton for Inventory. So, we're going to make a static call of Inventory, so we can only
    // ever have one instance of Inventory called.
    // So, you can access this \/ by calling Inventory.instance in another class.
    public static Inventory instance;

    void Awake()
    {
  		//Make GameManager a Singleton
  		if (instance == null)
  			instance = this;
  		else if (instance != this)
  			Destroy(gameObject);
    }

    #endregion

    public int space = 5;
    public List<Item> items = new List<Item>();
    private int index;

    public bool Add(Item item)
    {
        if (items.Count >= space)
        {
            // Send GUI message to Player here (todo), return false.
            return false;
        }
        // Otherwise we can pick up the item, return true.
        items.Add(item);
        return true;
    }

    public void Remove(Item item)
    {
        items.Remove(item);
    }

    //This method returns the list of objects for GUI to display
    public List<Item> getList()
    {
        return items;
    }

    public Item getObjectInHand(int atIndex)
    {
      return items[atIndex];
    }

    //Switches out held item for the one to the right in the list and returns the object to be displayed on the GUI
    public Item toggleHandRight()
    {
      index++;
      if(index>=space)
        index=0;
      return getObjectInHand(index);
    }

    //Switches out held item for the one to the left in the list and returns the object to be displayed on the GUI
    public Item toggleHandLeft()
    {
      index--;
      if(index<0)
        index=space-1;
      return getObjectInHand(index);
    }

    //Calls object in hand to use it
    public void useHeldItem()
    {
      //Write later
    }

    public void Sort()
    {
      //Write later
    }

}
