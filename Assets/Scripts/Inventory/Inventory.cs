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
    public GameObject handObject = null;
    protected Material handScript;
    public bool newlyHeldObject = false;
    public GameObject MementoType;

    //Variables for entire program. Changes depending on equipment player is wearing
    public static float strength = 1f;
    public int space = 5;

    //UI Images for all six of the Inventory objects
    public List<UIItemBox> InventoryBoxes = new List<UIItemBox>();
    public const int CYCLE_SIZE = 270;

    void Awake()
    {
  		//Make GameManager a Singleton
  		if (instance == null)
  			instance = this;
  		else if (instance != this)
  			Destroy(gameObject);

      player = Player.getPlayer().gameObject;

      //Initialize Mementos list
      foreach(Item inventoryItem in items) {
        if(items.Count > ItemMementos.Count) {
          ItemMementos.Add(null);
        }
      }

      //Initialize UI Boxes
  		RectTransform ItemsBox = GameObject.FindWithTag("ItemBox").GetComponent<RectTransform>();
      for(int box=0; box<items.Count; box++) {
			     InventoryBoxes.Add(ItemsBox.Find("Container " + (box+1)).GetComponent<UIItemBox>());
           InventoryBoxes[box].Initialize(45+((CYCLE_SIZE/items.Count)*box), box);
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
        //Update UI
        UpdateUI();
        return true;
    }

    protected bool Add(ConcreteItem itemObj)
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
          handObject = null;
          handScript = null;
          if(items.Count != 0) {
            do {
              toggleHandLeft();
            } while(handIndex > items.Count);
          }
        }
        else {
          makeHandObject();
          //Since the players' hand isn't moving, the UI has to be updated manually
          UpdateUI();
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

    public int getInventoryIndex() {
      return handIndex;
    }

    public void makeHandObject() {
      handObject = (GameObject) Instantiate(items[handIndex].concreteObject);
      handScript = handObject.GetComponent<Material>();
      handScript.PickedUp(player);
      ConcreteItem itemInHand = handScript as ConcreteItem;
      if(itemInHand) {
        if(ItemMementos[handIndex] != null) {
          ItemMementos[handIndex].setParent(handScript);
          //itemInHand.SetMemento(ItemMementos[handIndex]);
        }
      }
    }

    //Switches out held item for the one to the right in the list and returns the object to be displayed on the GUI
    public Item toggleHandRight()
    {
      if(items.Count>0) {
        //The 'as' function returns null if the operation is impossible. This checks if the item in your
        //hand is a ConcreteItem that can be placed in the inventory
        ConcreteItem itemInHand = handScript as ConcreteItem;
        if(itemInHand) {
          if(newlyHeldObject==true) {
            AddAt(handIndex, itemInHand);
            newlyHeldObject = false;
          }
        }
        //Destroy old handObject
        if(handObject != null)
          Destroy(handObject);
        //Move handObject
        handIndex++;
        if(handIndex>=items.Count) {
          handIndex=0;
        }
        makeHandObject();
        //Move UI Boxes
        moveUI(handIndex);
        UpdateUI();
        //Return object in hand for methods that require it
        return getObjectInHand();
      } else {
        return null;
      }
    }

    //Switches out held item for the one to the left in the list and returns the object to be displayed on the GUI
    public Item toggleHandLeft()
    {
      if(items.Count>0) {
        //The 'as' function returns null if the operation is impossible. This checks if the item in your
        //hand is a ConcreteItem that can be placed in the inventory
        ConcreteItem itemInHand = handScript as ConcreteItem;
        if(itemInHand) {
          if(newlyHeldObject==true) {
            AddAt(handIndex, itemInHand);
            newlyHeldObject = false;
          }
        }
        //Destroy old handObject
        if(handObject != null)
          Destroy(handObject);
        //Move handObject
        handIndex--;
        if(handIndex<0) {
          handIndex=items.Count-1;
        }
        makeHandObject();
        //Move UI Boxes
        moveUI(handIndex);
        UpdateUI();
        //Return object in hand for methods that require it
        return getObjectInHand();
      } else {
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
      //Update UI
      UpdateUI();
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
        //Update UI
        UpdateUI();
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
      if(index<items.Count && index>=0) {
        return items[index];
      }
      return null;
    }

    //Is this needed? Decide later
    public void Sort()
    {
      //Write later
    }

    public void SaveInventory(QuickLife script) {
      for(int i=0; i<items.Count; i++) {
        if(i == handIndex) {
          ConcreteItem itemInHand = handScript as ConcreteItem;
          if(itemInHand) {
              ItemMementos[i] = itemInHand.CreateInventoryMemento();
              //The item in your hand will already be saved in the QuickLife script by the timeVibration
            }
          } else {
            ItemMemento newMemento = Instantiate(MementoType).GetComponent<ItemMemento>();
            newMemento.InitializeInventory(true);
            script.SaveMemento(newMemento);
            //Place memento into a list of Mementos
            ItemMementos[i] = newMemento;
        }
      }
    }

    //Takes the object and places it in the inventory, destroying the concrete object
    //@error: Method raises an error if the concreteObject parameter is not compatible with the inventory
    public void TransferIn(int index, Material concreteObject) {
      ConcreteItem itemToRevert = concreteObject as ConcreteItem;
      if(itemToRevert) {
        itemToRevert.EmptyMemento();
        AddAt(index, itemToRevert);
        makeHandObject();
        //Destroy the concreteObject version
        Destroy(concreteObject.gameObject);
        //Update UI
        UpdateUI();
      } else {
        Debug.LogException(new Exception("Inventory Revert method expected a ConcreteItem object"), this);
      }
    }

  private void UpdateUI() {
    for(int box=0; box<InventoryBoxes.Count; box++) {
      InventoryBoxes[box].UpdateImage();
    }
  }

  //Move all boxes so handIndex box is at the front
  private void moveUI(int handIndex) {
    int curPlace = InventoryBoxes[handIndex].getDegrees();
    int degrees = 0;
    //Calculate direction and distance to that position
    if(Mathf.Abs(curPlace-45) <= Mathf.Abs(CYCLE_SIZE+45-curPlace)) {
      degrees = -(curPlace - 45);
    } else {
      degrees = CYCLE_SIZE+45-curPlace;
    }
    //Move UI Boxes
    foreach(UIItemBox UIBox in InventoryBoxes) {
      UIBox.addDegrees(degrees);
    }
  }

  private void resetUI(int numItems) {
    int targetDegrees = 0; int curPlace = 0; int degrees = 0;
    //Move UI Boxes
    for(int box=0; box<numItems; box++) {
      //Calculate desired position
      targetDegrees = 45+box*(CYCLE_SIZE/numItems);
      //Calculate distance to that position from the current position
      curPlace = InventoryBoxes[box].getDegrees();
      //Calculate direction and distance to that position
      if(Mathf.Abs(curPlace-45) <= Mathf.Abs(CYCLE_SIZE+45-curPlace)) {
        degrees = -(curPlace - 45);
      } else {
        degrees = CYCLE_SIZE+45-curPlace;
      }
      //Move it there
      InventoryBoxes[box].addDegrees(degrees);
    }
  }

}
