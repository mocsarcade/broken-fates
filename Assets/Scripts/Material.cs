using UnityEngine;
using System.Collections;
using System;

public class Material : MonoBehaviour {

    public const int SHADOW_CHANGE_RATE = 5;
    public const float FALL_VIBRATION_SIZE = 3f;
    //Weight is important. An object's weight can be from 1 to 3, with different levels
    //Of max throw height depending on how heavy the object is. An object of weight 3
    //Can only be thrown a third of the distance as an object of weight 1. vibrations
    //Are also made according to player's strength*weight
    public int weight;
    public int size;

    //Display variables
    public Transform myTransform;
    protected float z;
    //public SpriteRenderer renderer;
    public GameObject shadowObj;
    public Rigidbody2D rb2d;
    protected Shadow shadow;
    //PickedUp object holds the object holding this object. Otherwise, it is null
  	protected GameObject holder;

    //Mechanics-based Variables
    public GameObject MementoType;


    protected virtual void Awake() {
      myTransform = GetComponent<Transform>();
      rb2d = GetComponent<Rigidbody2D>();

      //This is the amount shadow has to be down by in order to be at the BOTTOM of the Material
      float y_offset = GetComponent<SpriteRenderer>().bounds.size.y/2;
      //Vector2 offset = Vector2.zero;
      //Initialize shadow to be a bigger blob depending on size of object and
      //give reference to transform so shadow will follow
      shadow = Instantiate(shadowObj, transform.position, Quaternion.identity).GetComponent<Shadow>();
      shadow.Initialize(myTransform, size, y_offset);
      holder = null;
      LoadMemento();
    }

    //Subclasses of Material will need Mementos that are more detailed.
    //This method encapsulates the Memento-choosing process
    protected virtual void LoadMemento() {
      MementoType = (GameObject) Resources.Load("BasicMemento");
      Debug.Log("Loading Memento!");
    }

    //Throw this object from x starting position to y target
    //This method is a coroutine
    public virtual IEnumerator Throw(Vector2 start, Vector2 target, float strength) {
      Drop();
      Vector2 myPosition = shadow.getPosition();
      Vector2 nextPosition = myPosition;
      float throwSpeed = (strength+3)/3;
      //Check that start is at least close to the object's actual position
      if(Vector2.Distance(myPosition, start) > 2) {
        Debug.LogException(new Exception("Throw method used TOO FAR from the actual object"), this);
        yield break;
      }
      float zRate = Mathf.Sqrt(9.8f*Vector2.Distance(start, target));
      float fullT = zRate/4.9f;
      float xyRate = Vector2.Distance(start, target)/fullT;
      float t = 0;
      //Deactivate Mobility and Shadow following
      shadow.Detach();
      //While not at target
      while(Vector2.Distance(myPosition,target) >= 0.05f)
      {
        //Get new Position
        //New position is decided by how powerful the throw is divided by the object's weight
        //A bottle would be weight 1, so a fast throw would have strength 2 or more
        nextPosition = Vector2.MoveTowards(myPosition, target, xyRate*Time.deltaTime*throwSpeed);

        //Move Shadow
        shadow.setPosition(nextPosition);
        //Move Me
        //Moving an extra bit of upwards to simulate jumping/height
        rb2d.MovePosition(nextPosition + (Vector2.up*z));
        //Change Z
        t += Time.deltaTime*throwSpeed;
        z = (zRate*t - 4.9f*t*t) * 2/(weight+1);
        //Update shadow depending on height from object to shadow
        if(z%SHADOW_CHANGE_RATE == 0) {
          shadow.UpdateSize((int) z);
        }

        //Yield until next frame
        yield return new WaitForFixedUpdate();
        myPosition = shadow.getPosition();
      }
      //Object has reached target, so make vibration
  		Vibration.Vibrator().MakeVibration((int) (throwSpeed*((weight+2)/3)*(9.8f*fullT - zRate)*FALL_VIBRATION_SIZE), (Vector2) transform.position, gameObject);
      //Now that we're done, reactivate Mobility
      shadow.Attach();
    }

    void OnDestroy() {
      if(shadow != null) {
        Destroy(shadow.gameObject);
      }
    }

    //The "virtual" is important to show this method will be overriden
  	//This method is called whenever a vibration touches a moving object in the BlockingLayer collision layer
  	public virtual void FeelVibration (Vector2 sourcePosition) {}

    //Picks up this object and returns either an "Item" asset object for concreteItem objects
    //or null, telling the program the Use() function cannot be done on the item in the players' hand
    public virtual Item PickedUp(GameObject _holder) {
      holder = _holder;
      //Put the items together
      if(_holder != null) {
        Transform newPosition = _holder.transform;
    		transform.position = newPosition.position;
        transform.parent = newPosition;
      } else {
        transform.parent = null;
      }
      //NOTE FOR LATER: Create functionality to attach this object to the player and move where the player does,
      //so it looks like "holding" the object. Current code just makes it follow player awkwardly

      //Return null so this object cannot be "Used" like an item; only thrown
      return null;
    }

    public virtual void PickUp(GameObject item) {
      item.GetComponent<Material>().PickedUp(gameObject);
    }

    public void Drop() {
      holder = null;
      transform.parent = null;
    }

    //Save State method for time blink. This method saves the object's state into an object
    public virtual Memento CreateMemento() {
      Memento newMemento = Instantiate(MementoType).GetComponent<Memento>();
      newMemento.Initialize(this);
      return newMemento;
    }

    //The second half of time blink. This method uses the object to reconstruct the object into its old form
    public virtual void useMemento(Memento oldState) {
      transform.position = oldState.position;
      //Take the object picking you up and call its "PickUp" method. Player's pick up method will call inventory
      //Monsters will just call the "pick up" method of the pick=up-ee
      if(holder != oldState.holder)
        oldState.holder.GetComponent<Material>().PickUp(gameObject);
    }

    public GameObject getHolder() {
      return holder;
    }

    	// Interface so inventory can know this object has no "use"
    	public virtual void Use () {}

}
