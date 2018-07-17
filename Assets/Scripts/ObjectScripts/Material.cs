using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MEC;

public class Material : MonoBehaviour {

	 public Memento curMemento;

    public const int SHADOW_CHANGE_RATE = 5;
    public const float FALL_VIBRATION_SIZE = 17f;

		protected string defSortingLayer;
		protected int defSortingOrder;
    //Weight is important. An object's weight can be from 1 to 3, with different levels
    //Of max throw height depending on how heavy the object is. An object of weight 3
    //Can only be thrown a third of the distance as an object of weight 1. vibrations
    //Are also made according to player's strength*weight
    public int weight;

    //Local GameObject variables
    protected Transform myTransform;
    protected Collider2D myCollider;
    protected SpriteRenderer myRenderer;
    protected bool destroyed;
    //public SpriteRenderer renderer;
    public GameObject shadowObj;
    protected Rigidbody2D rb2d;
    public Shadow shadow;
    //PickedUp object holds the object holding this object. Otherwise, it is null
  	public Material holder;
  	//protected GameObject thrower;

    //Runtime Variables
    protected GameObject MementoType;
    protected MementoData mementoData;
		public bool beingThrown;
		protected bool toReverse;
		protected Collider2D throwerCollider;
		public List<GameObject> damagedObjects = new List<GameObject>();


    protected virtual void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      myRenderer = GetComponent<SpriteRenderer>();

			//Initailize Sorting Layer, so when this object is dropped, it will return to its regular Layer
			defSortingLayer = myRenderer.sortingLayerName;
			defSortingOrder = myRenderer.sortingOrder;
      //Initialize shadow to be a bigger blob depending on size of object and
      //give reference to transform so shadow will follow
      shadow = Instantiate(shadowObj, transform.position, Quaternion.identity).GetComponent<Shadow>();
      shadow.Initialize(transform, weight, GetComponent<Collider2D>());

			//Set this object to be a child of the shadow
			transform.parent = shadow.gameObject.transform;

      myCollider = GetComponent<Collider2D>();//shadow.gameObject.GetComponent<Collider2D>();
			myTransform = shadow.gameObject.GetComponent<Transform>();

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
    }

		public void UpdatePosition(float z_offset) {//float x_position, float y_position, float z_offset) {
			if(shadow) {
				transform.localPosition = new Vector3(0, shadow.GetOffset() + z_offset, 0);//x_position, y_position + z_offset, 0);
			} else {
				transform.localPosition = new Vector3(0, z_offset, 0);//x_position, y_position + z_offset, 0);
			}
		}

    //Throw this object from x starting position to y target
    //This method is a coroutine
    public virtual IEnumerator<float> Throw(Vector2 target, int arcDegrees) {
			beingThrown = true;
			//Ignore collisions between this object and the thrower for a short amount of time
			if(holder) {
				throwerCollider = holder.GetCollider();
				Physics2D.IgnoreCollision(myCollider, throwerCollider, true);
			}
			//shadow.setPosition(transform.position);
      Drop();

			//Run-time variables
			Vector2 distance = target - (Vector2) GetPosition();

			float zRate = Mathf.Sqrt(9.8f*Vector2.Distance(GetPosition(), target));

			float arcRadians = arcDegrees*Mathf.PI/180;
			float factor = Mathf.Sin(arcRadians)/Mathf.Sin(Mathf.PI/4);

			shadow.addVelocity((zRate+(4.9f*Time.deltaTime))*Time.deltaTime*factor); //Arc is increased by a factor of 1/factor, multiplying distance by 1/factor
			distance = distance*(1f/factor); //By increasing distance by a multiple of inverse factor, the speed (and so distance) will be multiplied by factor
			//These two together cause distance to be the same; the distance to target, but it makes the speed and arc different

			shadow.PushDist(distance.normalized*distance.magnitude/(shadow.GetRigidbody().drag * (26f/15)), ForceMode2D.Impulse);
			//Wait for the object to come to a complete stop
			while(shadow.GetSpeed() > 0.1f) {
				UpdatePosition(shadow.GetHeight());

				yield return Timing.WaitForOneFrame;
			}
			//Object has reached target, so make vibration
			Vibration.Vibrator().MakeVibration((int) ((zRate)*FALL_VIBRATION_SIZE), (Vector2) GetPosition(), gameObject);

			//Object may have crashed into other objects and hurt them, so empty the damagedList as well
			EmptyDamagedList();

			//Object is no longer attached to thrower, so let it be able to collider with thrower again
			if(throwerCollider) {
				Physics2D.IgnoreCollision(myCollider, throwerCollider, false);
				throwerCollider = null;
			}
			beingThrown = false;
    }

		public void UpdateZ(float z) {
			//Update Order in Layer
			myRenderer.sortingOrder = (int)(-GetPosition().y + z);
			//Update shadow depending on height from object to shadow
			shadow.UpdateSize();
		}

		/*
		public void reverseThrow() {
			toReverse = true;

			//NOTE: IF WE DECIDE THAT THE PLAYER WILL BE HIT BY THROWN OBJECTS THAT BOUNCE OFF WALLS AND HIT THEM,
			//TURN THIS NEXT SECTION INTO ITS OWN METHOD AND HAVE THE SHADOW's REVERSE METHOD CALL IT

			if(throwerCollider) {
				Physics2D.IgnoreCollision(myCollider, throwerCollider, false);
				throwerCollider = null;
			}
		}*/

		//Collision Variable for subclasses
		//protected virtual void OnCollisionEnter2D(Collision2D collision)
		protected virtual void OnTriggerEnter2D(Collider2D collision)
		{
			Debug.Log("Objects touching!");
			//Check if the object's concreteForm (parent) is also colliding with our object (on same z)
			Material touchedObj = collision.gameObject.GetComponent<Material>();
			//If the object has a shadow component, damage the touched object
			if(touchedObj) {
				if(GetShadow().GetCollider().IsTouching(touchedObj.GetShadow().GetCollider())) {
					//If it is, damage it by a certain amount. That shadow object will do the same to this object
					Attack(touchedObj, GetDamageAmount());
					//touchedObj.Damage(GetDamageAmount(), gameObject);
					//Disable collision between these two objects until collision exits
					//Physics2D.IgnoreCollision(myCollider, touchedObj.GetCollider(), true);
				}
			}
		}

		//protected void OnCollisionExit2D(Collision2D collision)
		protected void OnTriggerExit2D(Collider2D collision)
		{
			Material touchedObj = collision.gameObject.GetComponent<Material>();
			if(touchedObj) {
				//Physics2D.IgnoreCollision(myCollider, touchedObj.GetCollider(), false);
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
    		SetPosition(newPosition.position, 0); //0 for now!
        shadow.setFollow(newPosition);

				//Disable pickedUp object colliding or moving of its own accord
				DisableRigidbody();
	      //Place object on Object's layer so it looks like "holding" the object
				myRenderer.sortingLayerName = _holder.GetComponent<SpriteRenderer>().sortingLayerName;
				myRenderer.sortingOrder = _holder.GetComponent<SpriteRenderer>().sortingOrder+1;

				Debug.Log(_holder + " picked UP " + gameObject);
      } else {
				shadow.setFollow(null);
      }
      //Return null so this object cannot be "Used" like an item; only thrown
      return null;
    }

		//Collision of Shadow is disabled
		public virtual void DisableRigidbody() {
			Debug.Log(gameObject.name + "Disabling Rigidbody!");
			shadow.GetRigidbody().bodyType = RigidbodyType2D.Kinematic;
			shadow.DisableCollider();
			myCollider.enabled = false;
		}

		//Collision of Shadow is enabled
		public virtual void EnableRigidbody() {
			Debug.Log(gameObject.name + "Enabling Rigidbody!");
			shadow.GetRigidbody().bodyType = RigidbodyType2D.Dynamic;
			shadow.EnableCollider();
			myCollider.enabled = true;
		}

    public void Drop() {
      holder = null;
      shadow.setFollow(null);
			//Reactivate rigidbody and collision
			EnableRigidbody();
			//Reset Sorting Layer
			myRenderer.sortingLayerName = defSortingLayer;
			myRenderer.sortingOrder = defSortingOrder;
    }

    public virtual void PickUp(GameObject item) {
      item.GetComponent<Material>().PickedUp(gameObject);
    }

		public void UpdatePositionAtHand() {
			if(holder != null) {
				//Create a variable to hold the size of this object divided by two so the exact center of this object will be held
		    Vector3 mySize = myRenderer.size/2;
				Vector3 newPosition = holder.GetHeldPosition(GetPosition());
				//float new_Z = holder.GetCollider().bounds.min.y - newPosition.y + holder.GetHeight();
				if(newPosition != GetPosition()-mySize) {
					float shadowCorrector = holder.GetShadow().GetSize().y/2 - GetShadow().GetSize().y;

					//Change position to fit hand
					shadow.setHeight(mySize.y + shadowCorrector);
					shadow.setObjectPosition(new Vector2(newPosition.x-mySize.x, newPosition.y - shadowCorrector));

					//Check that the item may have changed sorting order
					myRenderer.sortingOrder = holder.GetHeldSortingOrder();
				}
			}
		}

		public void SetPosition(Vector2 newPosition, int z) {
			shadow.setPosition(newPosition);
		}

		public Vector3 GetPosition() {
			return myTransform.position;
		}

		public virtual Vector3 GetHeldPosition(Vector3 oldPosition) {
			return oldPosition;
		}

		public virtual int GetHeldSortingOrder() {
			return myRenderer.sortingOrder+1;
		}

		/*
		public Vector2 GetColliderSize() {
			return myCollider.bounds.size;
		}*/

		public Collider2D GetCollider() {
			return myCollider;
		}

		public Shadow GetShadow() {
			return shadow;
		}

		public bool GetThrowState() {
			return beingThrown;
		}

		private void Detach() {
			transform.parent = null;
		}

		private void Attach() {
			transform.parent = shadow.transform;
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
			if(damageAmo > 0) {
				bool canDamage = CheckDamagedList(damaged.gameObject);
				if(canDamage) {
					damaged.Damage(damageAmo);
				}
			}
		}

		//Function that allows this object to take damage. Can take many forms. Only ever called by Attack(...)
		public virtual void Damage(float damageAmo) {}

		protected void EmptyDamagedList() {
			damagedObjects.Clear();
		}

		public virtual float GetDamageAmount() { return 0; }

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
      if(holder != oldState.holder && oldState.holder != null) {
        oldState.holder.PickUp(gameObject);
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

    public Material GetHolder() {return holder;}
    public Sprite GetSprite() {return myRenderer.sprite;}

    public float GetHeight() {return shadow.GetHeight();}
    public float GetZVelocity() {return shadow.GetHeightVelocity();}
    public Vector2 GetMomentum() {return shadow.GetMomentum();}

    public bool GetExists() {return !destroyed;}

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
