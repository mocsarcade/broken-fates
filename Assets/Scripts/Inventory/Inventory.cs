using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using MEC;

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
    public Image UIBoxPrefab;

    //Variables for entire program. Changes depending on equipment player is wearing
    public static float strength = 1f;
    public int space = 5;

    //UI Images for all six of the Inventory objects
    private RectTransform ItemsBox;
    public List<UIItemBox> InventoryBoxes = new List<UIItemBox>();
    public const float CYCLE_SIZE = 270f;

    void Awake()
    {
  		//Make GameManager a Singleton
  		if (instance == null)
  			instance = this;
  		else if (instance != this)
  			Destroy(gameObject);

      player = Player.GetPlayer().gameObject;

      //Initialize Mementos list
      foreach(Item inventoryItem in items) {
        if(items.Count > ItemMementos.Count) {
          ItemMementos.Add(null);
        }
      }

      //Initialize UI Boxes
  	  ItemsBox = GameObject.FindWithTag("ItemBox").GetComponent<RectTransform>();
      for(int box=0; box<items.Count; box++) {
          Image newBox = Instantiate(UIBoxPrefab);
          newBox.GetComponent<RectTransform>().SetParent(ItemsBox, false);
			    InventoryBoxes.Add(newBox.GetComponent<UIItemBox>());
          InventoryBoxes[box].Initialize(45f+((CYCLE_SIZE/items.Count)*(box-handIndex)), box);
      }

      LoadMemento();
    }

    void Start() {
      makeHandObject();
    }

    private void LoadMemento() {
  		MementoType = (GameObject) Resources.Load("ItemMemento");
  	}

    void Update() {
      if(GlobalRegistry.CheckKey ("ToggleL"))
      {
        toggleHand(-1);
      }
        if(GlobalRegistry.CheckKey ("ToggleR"))
      {
        toggleHand(1);
      }
      if(GlobalRegistry.CheckKey ("Use")) {useHeldItem();}
    }

    //This is the dungeon-crawl item addition, used when picking up objects to throw
    public bool PickUp(GameObject obj) {
      if (items.Count < space)
      {
        //If there is no "new" object in players' hand:
        if(newlyHeldObject == false) {
          if(handObject != null)
            Destroy(handObject);
          handObject = obj;
          handScript = obj.GetComponent<Material>();
          if(handScript) {
            //Tell item it has been picked up
            handScript.PickedUp(Player.GetPlayer().gameObject);
            newlyHeldObject = true;
            return true;
          } else {
            return false;
          }
        } else {
          //NOTE: Later make this try to move object in hand into the players' hand, and return false only if that fails.
          return false;
        }
      } else {
        return false;
      }
    }

    //This is the dungeon-crawl item addition, used when picking up items from chests. It automatically adds that object to your hand
    public bool PickUp(Item item) {
      if (items.Count < space)
      {
        //If there is no "new" object in players' hand:
        if(newlyHeldObject == false) {
          makeObject(item);
          newlyHeldObject = true;
          return true;
        } else {
          //NOTE: Later make this try to move object in hand into the players' inventory, and automatically add item to inventory otherwise.
          return false;
        }
      } else {
        return false;
      }
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
        //Create a new ItemBox at index Count (end of list)-1 to parallel this item added at the end
        CreateItemBox(items.Count-1);
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
        ItemMementos.Add(itemObj.GetMemento() as ItemMemento);
        //Create a new ItemBox at index Count (end of list)-1 to parallel this item added at the end
        CreateItemBox(items.Count-1);
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
        ItemMemento tempMemento = itemObj.GetMemento() as ItemMemento;
        if(tempMemento != null) {
          itemObj.setInInventory(true, handIndex);
          tempMemento.setParent(null);
          ItemMementos.Insert(index, tempMemento);
        } else {
          ItemMementos.Insert(index, null);
        }
        //Create a new ItemBox at index
        CreateItemBox(index);
        //Destroy old handObject to make new one
        if(handObject != null)
          Destroy(handObject);
        makeHandObject();
        return true;
    }

    //Remove this object from the inventory. This only removes it; it doesn't destroy the concreteItem object
    public void Remove(int index)
    {
      //If the item has a memento, update it. Setting the memento's parent will require each operation to change it during the operation call
      if(items[index] != null) {
        if(ItemMementos[index] != null) {
          ItemMementos[index].setInInventory(false, index);
        }
        if(ItemMementos.Count >= items.Count) {
          ItemMementos.RemoveAt(index);
        }
        items.RemoveAt(index);
        RemoveItemBox(index);
        //If there is no longer any handObject, remake it
        if(handIndex >= items.Count) {
          handObject = null;
          handScript = null;
          Debug.Log("We are decreasing handIndex!");
          //If the removed item was the last item
          if(items.Count != 0) {
            do {
              Debug.Log("Looping!");
              toggleHand(-1);
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

    //Get Methods
    public Item GetObjectInHand() {return items[handIndex];}
    public List<Item> GetList() {return items;}
    public int itemsInInventory() {return items.Count;}
    public int GetInventoryIndex() {return handIndex;}

    public void makeHandObject() {
      makeObject(items[handIndex]);
      ConcreteItem itemInHand = handScript as ConcreteItem;
      if(itemInHand && ItemMementos.Count > handIndex) {
        //Update the item's memento, to tell it there is now a concrete version of it in existence
        if(ItemMementos[handIndex] != null) {
          ItemMementos[handIndex].setParent(handScript);
          //itemInHand.SetMemento(ItemMementos[handIndex]);
        }
        //Set curMemento
        handScript.setMemento(ItemMementos[handIndex]);
      }
    }

    public void makeObject(Item item) {
      handObject = (GameObject) Instantiate(item.concreteObject, transform.position, Quaternion.identity);
      handScript = handObject.GetComponent<Material>();
      handObject.transform.parent = player.transform;
      //Update object's position to be in the player's hand
      //handScript.UpdatePositionAtHand();
    }

    //Switches out held item for the one to the right in the list and returns the object to be displayed on the GUI
    public Item toggleHand(int directionAmount)
    {
      if(directionAmount == 0) {
        return null;
      }
      //Check to see if this toggle function is moving from a null area
      if(handIndex >= items.Count && items.Count > 0) {
        //Change directionAmount to either 1 or -1
        directionAmount = directionAmount/Mathf.Abs(directionAmount);
        do {
          handIndex += directionAmount;
          if(handIndex>=items.Count)
            handIndex=0;
          if(handIndex<0)
            handIndex=items.Count-1;
        } while(handIndex >= items.Count);
      } else {
        //The 'as' function returns null if the operation is impossible. This checks if the item in your
        //hand is a ConcreteItem that can be placed in the inventory
        ConcreteItem itemInHand = handScript as ConcreteItem;
        if(itemInHand) {
          if(newlyHeldObject==true) {
            AddAt(handIndex, itemInHand);
            newlyHeldObject = false;
          } else if(items.Count>0) {
            //Move handObject
            handIndex += directionAmount;
            if(handIndex>=items.Count)
              handIndex=0;
            if(handIndex<0)
              handIndex=items.Count-1;
          } else {
            //If neither the object in hand is added to the inventory, nor are there any items left,
            //Return null
            return null;
          }
        } else {
          //If the hand object isn't an item, return null
          return null;
        }
      }
      //If either of the above options was activated, reset the handObject and update the UI
      //Destroy old handObject
      if(handObject != null)
        Destroy(handObject);
      makeHandObject();
      //Move UI Boxes
      moveUI(handIndex);
      UpdateUI();
      //Return object in hand for methods that require it
      return GetObjectInHand();
    }

    //Calls object in hand to use it
    public void useHeldItem()
    {
      handScript.Use();
      //Update UI
      UpdateUI();
    }

    //If the object is a one-use, the concreteItem method will call this method to destroy it
    public void destroyHandObject() {
      ItemMemento handMemento = handScript.GetMemento() as ItemMemento;
      if(handMemento) {
        handMemento.setInInventory(false, 0);
      }
      if(newlyHeldObject == true) {
        handScript.Destroy();
        makeHandObject();
        newlyHeldObject = false;
      } else {
        handScript.Destroy();
        Remove(handIndex);
      }
    }

    //Calls object in hand to use it
    public void throwHeldItem(Vector2 tarGet)
    {
      if(handObject != null) {
        Timing.RunCoroutine(handObject.GetComponent<Material>().Throw(tarGet, 30).CancelWith(handObject), Segment.FixedUpdate);
        if(newlyHeldObject == true) {
          handObject = null;
          handScript = null;
          if(itemsInInventory() > 0)
            makeHandObject();
          newlyHeldObject = false;
        }
        else {
          //If this was an object in your inventory, update Memento
          ItemMemento tempMemento = handScript.GetMemento() as ItemMemento;
          if(tempMemento != null) {
            tempMemento.setParent(handScript);
            tempMemento.setInInventory(false, handIndex);
          }
          //If object was an object from the inventory, remove it from the inventory
          Remove(handIndex);
        }
        //Update UI
        UpdateUI();
      }
    }

    public float GetStrength() {
      return strength;
    }

    public bool isHolding() {
      if(handObject != null) {
        return true;
      }
      return false;
    }

    public int GetWeight() {
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
              script.SaveMemento(ItemMementos[i]);
            }
          } else {
            ItemMemento newMemento = Instantiate(MementoType).GetComponent<ItemMemento>();
            newMemento.InitializeInventory(i);
            script.SaveMemento(newMemento);
            //Place memento into a list of Mementos
            ItemMementos[i] = newMemento;
        }
      }
    }

    //Takes the object and places it in the inventory, destroying the concrete object
    //@error: Method raises an error if the concreteObject parameter is not compatible with the inventory
    public void TransferIn(int index, Material concreteObject) {
      Debug.Log("Transferring object back in!");
      ConcreteItem itemToRevert = concreteObject as ConcreteItem;
      if(itemToRevert) {
        itemToRevert.EmptyMemento();
        AddAt(index, itemToRevert);
        //Destroy the concreteObject version
        concreteObject.Destroy();
        //Update UI
        UpdateUI();
      } else {
        Debug.LogException(new Exception("Inventory Revert method expected a ConcreteItem object"), this);
      }
    }

  private void CreateItemBox(int index) {
    //Create object
    Image newBox = Instantiate(UIBoxPrefab);
    //Set its size to 0
    RectTransform boxTransform = newBox.gameObject.GetComponent<RectTransform>();
    boxTransform.sizeDelta = new Vector2(0,0);
    //Initialize degrees and place in InventoryBoxes list
    newBox.GetComponent<RectTransform>().SetParent(ItemsBox, false);
    InventoryBoxes.Insert(index, newBox.GetComponent<UIItemBox>());
    InventoryBoxes[index].Initialize(45+((CYCLE_SIZE/items.Count)*(index-handIndex)), index);
    //Start to increase its size to the regular amount
    InventoryBoxes[index].resizeUIImage(50);
    //Update all future boxes after index so the handIndex variable goes up
    for(int box=index+1; box<InventoryBoxes.Count; box++) {
      //Increase handIndex by 1
      InventoryBoxes[box].UpdateHandIndex(1);
    }
    resetUI(InventoryBoxes.Count);
  }

  // Enlarge the mechanics icon when the menu button is held, so it is clear which menu is being shifted
	private void DestroyItemBox(int boxIndex) {
    //Shrink the object into oblivion!
    InventoryBoxes[boxIndex].resizeUIImage(0);
    InventoryBoxes[boxIndex].setToDestroy();
  }

private void RemoveItemBox(int index) {
  //Update all future boxes after index so the handIndex variable goes up
  for(int box=index+1; box<InventoryBoxes.Count; box++) {
    //Increase handIndex by 1
    InventoryBoxes[box].UpdateHandIndex(-1);
  }
  //Destroy item box
  DestroyItemBox(index);
  InventoryBoxes.RemoveAt(index);
  resetUI(InventoryBoxes.Count);
}

  private void UpdateUI() {
    for(int box=0; box<InventoryBoxes.Count; box++) {
      InventoryBoxes[box].UpdateImage();
    }
  }

  //Move all boxes so handIndex box is at the front
  private void moveUI(int handIndex) {
    float curPlace = InventoryBoxes[handIndex].GetDegrees();
    float degrees = 0;
    //Calculate direction and distance to that position
    if(Mathf.Abs(curPlace-45) <= Mathf.Abs(CYCLE_SIZE+45-curPlace)) {
      degrees = -(curPlace - 45);
    } else {
      degrees = (float) CYCLE_SIZE+45-curPlace;
    }
    //Move UI Boxes
    foreach(UIItemBox UIBox in InventoryBoxes) {
      UIBox.addDegrees(degrees);
    }
  }

  private void resetUI(int numItems) {
    float tarGetDegrees = 0; float curPlace = 0; float degrees = 0;
    //Move UI Boxes
    for(int box=0; box<numItems; box++) {
      //Calculate desired position
      tarGetDegrees = 45+(box-handIndex)*(CYCLE_SIZE/numItems);
  		//If the tarGet is over the limit, it loops around to enter on the other side
  		if(tarGetDegrees>=180) {
  			tarGetDegrees -= 270;
  		} else if(tarGetDegrees<-90) {
  			tarGetDegrees += 270;
  		}
      //Calculate distance to that position from the current position
      curPlace = InventoryBoxes[box].GetDegrees();
      //Calculate direction and distance to that position
      if((Mathf.Max(curPlace, tarGetDegrees)-Mathf.Min(curPlace, tarGetDegrees)) <= (CYCLE_SIZE-(Mathf.Max(curPlace, tarGetDegrees)-Mathf.Min(curPlace, tarGetDegrees)))) {
        degrees = -(CYCLE_SIZE-(Mathf.Max(curPlace, tarGetDegrees)-Mathf.Min(curPlace, tarGetDegrees)));
      } else {
        degrees = (Mathf.Max(curPlace, tarGetDegrees)-Mathf.Min(curPlace, tarGetDegrees));
      }

  		//If the tarGet is over the limit, it loops around to enter on the other side
  		if(degrees>=180) {
        while(degrees>=180)
  			   degrees -= 270;
  		} else if(degrees<-90) {
        while(degrees<-90)
  			   degrees += 270;
  		}
      if(curPlace >= tarGetDegrees) {
        degrees = -degrees;
      }
      //Move it there
      InventoryBoxes[box].addDegrees(degrees);
    }
  }

}
