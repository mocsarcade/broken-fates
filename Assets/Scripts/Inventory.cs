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
        if (instance != null)
        {
            Debug.LogWarning("More than one inventory existing.");
        }
        instance = this;
    }

    #endregion

    public int space = 10;
    public List<Item> items = new List<Item>();

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

}
