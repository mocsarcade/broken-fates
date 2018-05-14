using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    #region Singleton
    // We're going to create a singleton for Inventory. So, we're going to make a static call of Inventory, so we can only
    // ever have one instance of Inventory called.
    // So, you can access this \/ by calling Inventory.instance in another class.
    public static Inventory instance;
    public GameObject player;

    void Awake()
    {
  		//Make GameManager a Singleton
  		if (instance == null)
  			instance = this;
  		else if (instance != this)
  			Destroy(gameObject);

      player = GameObject.FindGameObjectWithTag("Player");
      makeHandObject();
    }

    #endregion

    public List<Item> items = new List<Item>();
    private int handIndex;
    private GameObject handObject = null;
    private ConcreteItem handScript;

    //Variables for entire program. Changes depending on equipment
    public static float strength = 1f;
    public int space = 5;

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
        if(items.IndexOf(item) == handIndex) {
          toggleHandRight();
        }
        items.Remove(item);
    }

    public void Remove(int index)
    {
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
      handScript = handObject.GetComponent<ConcreteItem>();
      handScript.Initialize(player);
    }

    //Switches out held item for the one to the right in the list and returns the object to be displayed on the GUI
    public Item toggleHandRight()
    {
      if(handObject != null)
        Destroy(handObject);
      handIndex++;
      if(handIndex>=items.Count)
        handIndex=0;
      makeHandObject();
      return getObjectInHand();
    }

    //Switches out held item for the one to the left in the list and returns the object to be displayed on the GUI
    public Item toggleHandLeft()
    {
      Destroy(handObject);
      handIndex--;
      if(handIndex<0)
        handIndex=items.Count-1;
      makeHandObject();
      return getObjectInHand();
    }

    //Calls object in hand to use it
    public void useHeldItem()
    {
      handScript.Use();
    }

    //Calls object in hand to use it
    public void throwHeldItem(Vector2 start, Vector2 target)
    {
      if(handObject != null) {
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

    //Is this needed? Decide later
    public void Sort()
    {
      //Write later
    }

}
