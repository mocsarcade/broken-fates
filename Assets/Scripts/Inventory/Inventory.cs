using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour {

    // We're going to create a singleton for Inventory. So, we're going to make a static call of Inventory, so we can only
    // ever have one instance of Inventory called.
    // So, you can access this \/ by calling Inventory.instance in another class.
    public static Inventory instance;
    public GameObject player;

    public List<Item> items = new List<Item>();
    public List<ItemMemento> ItemMementos = new List<ItemMemento>();
    public int handIndex; //This variable should be protected. Only public for troubleshooting reasons. Turn protected later
    protected GameObject handObject = null;
    protected Material handScript;
    protected bool newlyHeldObject = false;
    public GameObject MementoType;

    //Variables for entire program. Changes depending on equipment player is wearing
    public static float strength = 1f;
    public int space = 5;

    void Awake()
    {
  		//Make GameManager a Singleton
  		if (instance == null)
  			instance = this;
  		else if (instance != this)
  			Destroy(gameObject);

      player = GameObject.FindGameObjectWithTag("Player");

      foreach(Item inventoryItem in items) {
        if(items.Count > ItemMementos.Count) {
          ItemMementos.Add(null);
        }
      }

      LoadMemento();
      makeHandObject();
    }

    private void LoadMemento() {
  		MementoType = (GameObject) Resources.Load("ItemMemento");
  	}

    void Update() {
      if(Input.GetButtonDown("ToggleL"))
      {
        toggleHandLeft();
      }
        if(Input.GetButtonDown("ToggleR"))
      {
        toggleHandRight();
      }
    }

    //This is the dungeon-crawl item addition, used when picking up items to throw, or finding something on the floor
    public void PickUp(GameObject obj) {
      handObject = obj;
      handScript = obj.GetComponent<Material>();
      //Tell item it has been picked up
      handScript.PickedUp(Player.getPlayer().gameObject);
      newlyHeldObject = true;
    }

    //This is the formal addition to the inventory, used in shops and dialogue-triggering items
    public bool Add(Item item)
    {
        if (items.Count >= space)
        {
            // Send GUI message to Player here (todo), return false.
            return false;
        }
        // Otherwise we can pick up the item, return true.
        items.Add(item);
        ItemMementos.Add(null);
        return true;
    }

    //This is the formal addition to the inventory, used in shops and dialogue-triggering items
    public bool Add(ConcreteItem itemObj)
    {
        Item item  = itemObj.GetItem();
        if (items.Count >= space)
        {
            // Send GUI message to Player here (todo), return false.
            return false;
        }
        // Otherwise we can pick up the item, return true.
        items.Add(item);
        ItemMementos.Add(itemObj.GetMemento());
        return true;
    }

    //This is a secondary add tool used when a PickedUp item is toggled left or right
    protected bool AddAt(int index, ConcreteItem itemObj)
    {
        Item item  = itemObj.GetItem();
        if (items.Count >= space)
        {
            // Send GUI message to Player here (todo), return false.
            return false;
        }
        // Otherwise we can pick up the item, return true.
        items.Insert(index, item);
        //If item has a memento, update it to know the object is now in the players' inventory
        ItemMemento tempMemento = itemObj.GetMemento();
        if(tempMemento != null) {
          tempMemento.setParent(null);
          tempMemento.setInInventory(true, handIndex);
          ItemMementos.Insert(index, tempMemento);
        } else  {
          ItemMementos.Insert(index, null);
        }
        return true;
    }

    /*
    public void Remove(Item item)
    {
        if(items.IndexOf(item) == handIndex) {
          toggleHandRight();
        }
        items.Remove(item);
    }*/

    public void Remove(int index)
    {
      //If the item has a memento, update it. Setting the memento's parent will require each operation to change it
      if(items[index] != null) {
        if(ItemMementos[index] != null) {
          ItemMementos[index].setInInventory(false, index);
        }
        if(ItemMementos.Count >= items.Count) {
          ItemMementos.RemoveAt(index);
        }
        items.RemoveAt(index);
        //If the removed item was in the players' hand and the last in the players' inventory
        if(index == handIndex && index >= items.Count) {
          //If the removed item was the last item
          if(items.Count == 0) {
            handObject = null;
            handScript = null;
          } else {
            toggleHandRight();
          }
        }
        else {
          makeHandObject();
        }
      }
    }

    //This method returns the list of objects for GUI to display
    public List<Item> getList()
    {
        return items;
    }

    public Item getObjectInHand()
    {
      return items[handIndex];
    }

    public int itemsInInventory() {
      return items.Count;
    }

    public void makeHandObject() {
      handObject = (GameObject) Instantiate(items[handIndex].concreteObject);
      handScript = handObject.GetComponent<Material>();
      handScript.PickedUp(player);
      ConcreteItem itemInHand = handScript as ConcreteItem;
      if(itemInHand) {
        if(ItemMementos[handIndex] != null) {
          ItemMementos[handIndex].setParent(handScript);
          itemInHand.SetMemento(ItemMementos[handIndex]);
        }
      }
    }

    //Switches out held item for the one to the right in the list and returns the object to be displayed on the GUI
    public Item toggleHandRight()
    {
      //The 'as' function returns null if the operation is impossible. This checks if the item in your
      //hand is a ConcreteItem that can be placed in the inventory
      ConcreteItem itemInHand = handScript as ConcreteItem;
      if(itemInHand) {
        if(newlyHeldObject==true) {
          AddAt(handIndex, itemInHand);
          newlyHeldObject = false;
        }
      if(handObject != null)
        Destroy(handObject);
      handIndex++;
      if(handIndex>=items.Count)
        handIndex=0;
      makeHandObject();
      return getObjectInHand();
      }
      else {
        return null;
      }
    }

    //Switches out held item for the one to the left in the list and returns the object to be displayed on the GUI
    public Item toggleHandLeft()
    {
      //The 'as' function returns null if the operation is impossible. This checks if the item in your
      //hand is a ConcreteItem that can be placed in the inventory
      ConcreteItem itemInHand = handScript as ConcreteItem;
      if(itemInHand) {
        if(newlyHeldObject==true) {
          AddAt(handIndex, itemInHand);
          newlyHeldObject = false;
        }
      if(handObject != null)
        Destroy(handObject);
      handIndex--;
      if(handIndex<0)
        handIndex=items.Count-1;
      makeHandObject();
      return getObjectInHand();
      }
      else {
        return null;
      }
    }

    //Calls object in hand to use it
    public void useHeldItem()
    {
      handScript.Use();
      //If one-use item, set newlyHeldObject=0
        //--- to write later---
          //If the item has a memento, update it
    }

    //Calls object in hand to use it
    public void throwHeldItem(Vector2 start, Vector2 target)
    {
      if(handObject != null) {
        if(newlyHeldObject == true)
          newlyHeldObject = false;
        else {
          //If this was an object in your inventory, update Memento
          ConcreteItem itemInHand = handScript as ConcreteItem;
          ItemMemento tempMemento = itemInHand.GetMemento();
          if(tempMemento != null) {
            tempMemento.setParent(handScript);
            tempMemento.setInInventory(false, handIndex);
          }
        }
        StartCoroutine(handObject.GetComponent<Material>().Throw(start, target, strength));
        Remove(handIndex);
      }
    }

    public float getStrength() {
      return strength;
    }

    public int getWeight() {
      //If inventory isn't empty
      if(handObject != null) {
        return handObject.GetComponent<Material>().weight;
      }
      return -1;
    }

    public Item GetItem(int index) {
      return items[index];
    }

    //Is this needed? Decide later
    public void Sort()
    {
      //Write later
    }

    public void SaveInventory(QuickLife script) {
      for(int i=0; i<items.Count; i++) {
        ItemMemento newMemento = Instantiate(MementoType).GetComponent<ItemMemento>();
        newMemento.InitializeInventory(true);
        script.SaveMemento(newMemento);
        //Place memento into a list of Mementos
        ItemMementos[i] = newMemento;
        if(i == handIndex) {
          ConcreteItem itemInHand = handScript as ConcreteItem;
          if(itemInHand)
            itemInHand.SetMemento(newMemento);
        }
      }
    }

    //Takes the object and places it in the inventory, destroying the concrete object
    //@error: Method raises an error if the concreteObject parameter is not compatible with the inventory
    public void TransferIn(int index, Material concreteObject) {
      ConcreteItem itemToRevert = concreteObject as ConcreteItem;
      if(itemToRevert) {
        itemToRevert.SetMemento(null);
        AddAt(index, itemToRevert);
        Destroy(concreteObject.gameObject);
      } else {
        Debug.LogException(new Exception("Inventory Revert method expected a ConcreteItem object"), this);
      }
    }

}
