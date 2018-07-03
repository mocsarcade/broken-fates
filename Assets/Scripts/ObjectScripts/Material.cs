using UnityEngine;
using System.Collections;
using System;

public class Material : MonoBehaviour {

	 public Memento curMemento;

    public const int SHADOW_CHANGE_RATE = 5;
    public const float FALL_VIBRATION_SIZE = 4f;
		public string defSortingLayer;
		public int defSortingOrder;
    //Weight is important. An object's weight can be from 1 to 3, with different levels
    //Of max throw height depending on how heavy the object is. An object of weight 3
    //Can only be thrown a third of the distance as an object of weight 1. vibrations
    //Are also made according to player's strength*weight
    public int weight;
    public int size;

    //Local GameObject variables
    protected Transform myTransform;
    protected Collider2D myCollider;
    protected SpriteRenderer myRenderer;
    public bool destroyed;
    //public SpriteRenderer renderer;
    public GameObject shadowObj;
    protected Rigidbody2D rb2d;
    public Shadow shadow;
    //PickedUp object holds the object holding this object. Otherwise, it is null
  	protected Material holder;
  	protected GameObject thrower;

    //Runtime Variables
    protected GameObject MementoType;
    protected MementoData mementoData;
		public bool beingThrown;
		protected bool toReverse;


    protected virtual void Awake() {
      myTransform = GetComponent<Transform>();
      rb2d = GetComponent<Rigidbody2D>();
      myRenderer = GetComponent<SpriteRenderer>();
      myCollider = GetComponent<Collider2D>();

      //This is the amount shadow has to be down by in order to be at the BOTTOM of the Material
      float y_offset = myRenderer.bounds.size.y/2;

			//Initailize Sorting Layer, so when this object is dropped, it will return to its regular Layer
			defSortingLayer = myRenderer.sortingLayerName;
			defSortingOrder = myRenderer.sortingOrder;
      //Initialize shadow to be a bigger blob depending on size of object and
      //give reference to transform so shadow will follow
      shadow = Instantiate(shadowObj, transform.position, Quaternion.identity).GetComponent<Shadow>();
      shadow.Initialize(myTransform, size, y_offset);
      holder = null;
      LoadMemento();
    }

		protected virtual void Update() {
			UpdatePositionAtHand();
		}

    //Subclasses of Material will need Mementos that are more detailed.
    //This method encapsulates the Memento-choosing process
    protected virtual void LoadMemento() {
      MementoType = (GameObject) Resources.Load("BasicMemento");
      Debug.Log("Loading Memento!");
    }

    //Throw this object from x starting position to y target
    //This method is a coroutine
    public virtual IEnumerator Throw(Vector2 target, float strength) {
			beingThrown = true;
			//Update thrower variable so this object knows what threw it
			//This is useful in subclasses where collision with a thrown object causes damage to all except the thrower
			if(holder) {
				thrower = holder.gameObject;
			} else {
				thrower = null;
			}
      Drop();

      Vector2 myPosition = shadow.getPosition();
      Vector2 nextPosition = myPosition;
			Vector2 start = myPosition;
			//Define Local Variables
			float z = 0;
			float colliderOffset_y = myCollider.offset.y;
      float throwSpeed = (strength+3)/3;
			float zRate = Mathf.Sqrt(9.8f*Vector2.Distance(start, target));
			float fullT = zRate/4.9f;
			//float xyRate = Vector2.Distance(start, target)/fullT;
			//Speed shadow moves, that the object follows
			//float moveRate = xyRate*throwSpeed*Time.deltaTime;
      float t = 0;
      //Deactivate Mobility and Shadow following
      shadow.Detach();
      //While not at target
      while(t < fullT)
      {
				//If the tag has been activated to bounce the object, reverse it
				if(toReverse == true) {
					target = myPosition - (target - myPosition);
					toReverse = false;
				}
        //Get new Position
        //New position is decided by how powerful the throw is divided by the object's weight
        //A bottle would be weight 1, so a fast throw would have strength 2 or more
        nextPosition = Vector2.Lerp(start, target, t/fullT);

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
				//Update Collider depending on height
				myCollider.offset = new Vector2(myCollider.offset.x, colliderOffset_y-(z/4));

        //Yield until next frame
        yield return new WaitForFixedUpdate();
        myPosition = shadow.getPosition();
      }
      //Object has reached target, so make vibration
  		Vibration.Vibrator().MakeVibration((int) (throwSpeed*((weight+2)/3)*(9.8f*fullT - zRate)*FALL_VIBRATION_SIZE), (Vector2) transform.position, gameObject);
      //Now that we're done, reactivate Mobility
      shadow.Attach();
			//Bring offset back to its initial amount
			myCollider.offset = new Vector2(myCollider.offset.x, colliderOffset_y);

			thrower=null;
			beingThrown = false;
    }

		protected void reverseThrow() {
			toReverse = true;
		}

		//When this thrown object bounces
		protected virtual void OnCollisionEnter2D(Collision2D collision)
		{
			if(beingThrown) {
				reverseThrow();
			}
		}

    //The "virtual" is important to show this method will be overriden
  	//This method is called whenever a vibration touches a moving object in the BlockingLayer collision layer
  	public virtual void FeelVibration (Vector2 sourcePosition) {}

    //Picks up this object and returns either an "Item" asset object for concreteItem objects
    //or null, telling the program the Use() function cannot be done on the item in the players' hand
    public virtual Item PickedUp(GameObject _holder) {
      //Put the items together
      if(_holder != null) {
				holder = _holder.GetComponent<Material>();
        Transform newPosition = _holder.transform;
    		transform.position = newPosition.position;
        transform.parent = newPosition;

				//Disable collider
	      myCollider.enabled = false;
	      //Place object on Object's layer so it looks like "holding" the object
				myRenderer.sortingLayerName = _holder.GetComponent<SpriteRenderer>().sortingLayerName;
				myRenderer.sortingOrder = _holder.GetComponent<SpriteRenderer>().sortingOrder+1;
      } else {
        transform.parent = null;
      }
      //Return null so this object cannot be "Used" like an item; only thrown
      return null;
    }

    public virtual void PickUp(GameObject item) {
      item.GetComponent<Material>().PickedUp(gameObject);
    }

		public void UpdatePositionAtHand() {
			if(holder != null) {
				//Create a variable to hold the size of this object divided by two so the exact center of this object will be held
		    Vector3 mySize = myRenderer.sprite.bounds.extents;
				mySize.y = -mySize.y;
				Vector3 newPosition = holder.GetHeldPosition(transform.position);
				if(newPosition != transform.position-mySize) {
					//Change position to fit hand
					transform.position = newPosition-mySize;
					//Check that the item may have changed sorting order
					myRenderer.sortingOrder = holder.GetHeldSortingOrder();
				}
			}
		}

		public virtual Vector3 GetHeldPosition(Vector3 oldPosition) {
			return oldPosition;
		}

		public virtual int GetHeldSortingOrder() {
			return myRenderer.sortingOrder+1;
		}

    public void Drop() {
      holder = null;
      transform.parent = null;
      myCollider.enabled = true;
			//Reset Sorting Layer
			myRenderer.sortingLayerName = defSortingLayer;
			myRenderer.sortingOrder = defSortingOrder;
    }

		public virtual void Damage(float damage) {}

    //Save State method for time blink. This method saves the object's state into an object
    public virtual Memento CreateMemento() {
  		//Keep track of item's memento so that should the item be placed in the inventory, it can be retrieved
  		if(curMemento == null) {
        mementoData = Instantiate((MementoData) GameManager.instance.getDataReference(GameManager.DataType.t_MementoData));
        Memento newMemento = Instantiate(MementoType).GetComponent<Memento>();
        newMemento.Initialize(this);
        return newMemento;
      } else {
    		curMemento.Initialize(this);
        return curMemento;
    	}
    }

    //The second half of time blink. This method uses the object to reconstruct the object into its old form
    public virtual void useMemento(Memento oldState) {
      transform.position = oldState.position;
      //Take the object picking you up and call its "PickUp" method. Player's pick up method will call inventory
      //Monsters will just call the "picked up" method of this object
      if(holder != oldState.holder && oldState.holder != null) {
        oldState.holder.PickUp(gameObject);
      }
			//Update Image to former image
			if(myRenderer.sprite != oldState.sprite) {
				myRenderer.sprite = oldState.sprite;
			}
    	//Empty ConcreteItem's curMemento variable
    	EmptyMemento();
    }

  	public Memento getMemento() {
  		return curMemento;
  	}

  	//Create method for when TimeVibration hits the object
  	public void setMemento(Memento newMemento) {
  		//Keep track of item's memento so that should the item be placed in the inventory, it can be retrieved
  		curMemento = newMemento;
  	}

  	public MementoData getMementoData() {
  		return mementoData;
  	}

  	public void EmptyMemento() {
  		curMemento = null;
      Destroy(mementoData);
  	}

    public Material getHolder() {
      return holder;
    }

    public Sprite getSprite() {
      return myRenderer.sprite;
    }

    public bool getExists() {
      return !destroyed;
    }

    public void Destroy() {
      destroyed = true;
      Drop();
      if(curMemento != null) {
        myRenderer.enabled = false;
      } else {
        Destroy(gameObject);
      }
    }

    void OnDestroy() {
      if(shadow != null) {
        Destroy(shadow.gameObject);
      }
    }

    public void Recreate() {
      destroyed = false;
      myRenderer.enabled = true;
    }

    // Interface so inventory can know this object has no "use"
    public virtual void Use () {}

}
