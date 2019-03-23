using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trap : MonoBehaviour {

	 //public Memento curMemento;

    public const int SHADOW_CHANGE_RATE = 5;
    public const float FALL_VIBRATION_SIZE = 25f;

		public const float AVERAGE_DELTA_TIME = 0.02f; //DeltaTime varies and by that causes the "throw" distance to...
		//well, fluctuate. Greater consistency is required, so we have provided a constant "average deltaTime"

		protected string defSortingLayer;
    //Weight is important. An object's weight can be from 1 to 3, with different levels
    //Of max throw height depending on how heavy the object is. An object of weight 3
    //Can only be thrown a third of the distance as an object of weight 1. vibrations
    //Are also made according to player's strength*weight

    //Local GameObject variables
    protected Transform myTransform;
    protected Collider2D myCollider;
    protected SpriteRenderer myRenderer;
    protected bool destroyed;
    //public SpriteRenderer renderer;
    public GameObject shadowObj;
    protected Rigidbody2D rb2d;
    //PickedUp object holds the object holding this object. Otherwise, it is null
  	//protected GameObject thrower;

    //Runtime Variables
    //protected GameObject MementoType;
    //protected MementoData mementoData;
		public bool beingThrown;
		protected bool toReverse;
		protected Vector3 _lastPosition;
		//protected Collider2D throwerCollider;
		protected Material throwerScript;
		public List<GameObject> damagedObjects = new List<GameObject>();


    protected virtual void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      myRenderer = GetComponent<SpriteRenderer>();
			myCollider = GetComponent<Collider2D>();//shadow.gameObject.GetComponent<Collider2D>();

				//Initailize Sorting Layer, so when this object is dropped, it will return to its regular Layer
				defSortingLayer = myRenderer.sortingLayerName;

			//LoadMemento();
		}

    //Subclasses of Material will need Mementos that are more detailed.
    //This method encapsulates the Memento-choosing process
		/*
    protected virtual void LoadMemento() {
      MementoType = (GameObject) Resources.Load("BasicMemento");
    }*/

		//Collision Variable for subclasses
		//protected virtual void OnCollisionEnter2D(Collision2D collision)
		protected virtual void OnTriggerEnter2D(Collider2D collision)
		{
			//Check if the object's concreteForm (parent) is also colliding with our object (on same z)
			//Material touchedObj = collision.gameObject.GetComponent<Material>();
			Debug.Log("Object touched");
		}

		protected virtual void Activate() {
			Debug.Log("Vibration touched");

		}

    //The "virtual" is important to show this method will be overriden
  	//This method is called whenever a vibration touches a moving object in the BlockingLayer collision layer
  	public virtual void FeelVibration (Vector2 sourcePosition) {
			Activate();
		}

		public Vector3 GetPosition() {
			return myTransform.position;
		}

		public Collider2D GetCollider() {
			return myCollider;
		}

		protected bool CheckDamagedList(GameObject damager) {
			if(damagedObjects.Contains(damager)) {
				return false;
			} else {
				damagedObjects.Add(damager);
				return true;
			}
		}

		//Attack function all objects use to damage other objects. Is the only function that calls the damage function
		public virtual void Attack(Material damaged, float damageAmo) {
			/*
			if(damageAmo > 0) {
				bool canDamage = CheckDamagedList(damaged.gameObject);
				if(canDamage) {
					damaged.Damage(damageAmo);
				}
			}
			*/
		}

		//Function that allows this object to take damage. Can take many forms. Only ever called by Attack(...)
		public virtual void Damage(float damageAmo) {
			
		}

		protected void EmptyDamagedList() {
			damagedObjects.Clear();
		}

		public virtual float GetDamageAmount() { return 0; }
		/*
    //Save State method for time blink. This method saves the object's state into an object
    public virtual Memento CreateMemento() {
  		//Keep track of item's memento so that should the item be placed in the inventory, it can be retrieved
  		if(curMemento == null) {
        mementoData = Instantiate((MementoData) GameManager.instance.GetDataReference(GameManager.DataType.t_MementoData));
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
      SetPosition(oldState.position, 0);
      //Take the object picking you up and call its "PickUp" method. Player's pick up method will call inventory
      //Monsters will just call the "picked up" method of this object
      if(holderData.Holder != oldState.holder && oldState.holder != null) {
				Material oldHolderScript = oldState.holder.GetComponent<Material>();
				if(oldHolderScript) {
					oldHolderScript.PickUp(gameObject);
				} else {
					PickedUp(oldState.holder);
				}
      }
			//Update Image to former image
			if(myRenderer.sprite != oldState.sprite) {
				myRenderer.sprite = oldState.sprite;
			}
			//Set height off ground and shadow velocity
			shadow.setHeight(oldState.z_offset, oldState.z_velocity);
			shadow.setMomentum(oldState.momentum);
    	//Empty ConcreteItem's curMemento variable
    	EmptyMemento();
    }

  	public Memento GetMemento() {
  		return curMemento;
  	}

  	//Create method for when TimeVibration hits the object
  	public void setMemento(Memento newMemento) {
  		//Keep track of item's memento so that should the item be placed in the inventory, it can be retrieved
  		curMemento = newMemento;
  	}

  	public MementoData GetMementoData() {
			return mementoData;
		}

  	public void EmptyMemento() {
  		curMemento = null;
      Destroy(mementoData);
  	}
		*/
    public Sprite GetSprite() {return myRenderer.sprite;}

    public bool GetExists() {return !destroyed;}

    public void Destroy() {
      destroyed = true;
      /*if(curMemento != null) {
        myRenderer.enabled = false;
      } else { */
        Destroy(gameObject);
      //}
    }

    public void Recreate() {
      destroyed = false;
      myRenderer.enabled = true;
    }
}
