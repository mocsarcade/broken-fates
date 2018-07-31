using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MEC;

public class Material : MonoBehaviour {

	 public Memento curMemento;

    public const int SHADOW_CHANGE_RATE = 5;
    public const float FALL_VIBRATION_SIZE = 17f;

		protected string defSortingLayer;
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
  	public HolderData holderData = new HolderData();
  	//protected GameObject thrower;

    //Runtime Variables
    protected GameObject MementoType;
    protected MementoData mementoData;
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
				//Initialize shadow to be a bigger blob depending on size of object and
				//give reference to transform so shadow will follow
				shadow = Instantiate(shadowObj, transform.position, Quaternion.identity).GetComponent<Shadow>();
				shadow.Initialize(transform, weight, GetComponent<Collider2D>());

				//Set this object to be a child of the shadow
				Transform oldParent = transform.parent;
				transform.parent = shadow.gameObject.transform;
				if(oldParent != null) {
					//shadow.setObjectPosition(transform.position);
					PickedUp(oldParent.gameObject, false);
				}
				//Set z to a random value so objects won't overlap and have a weird lighting effect
				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, - ((Random.value/100f) + (myRenderer.bounds.size.y/100f)));

				myTransform = shadow.gameObject.GetComponent<Transform>();

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
				transform.localPosition = new Vector3(0, shadow.GetOffset() + z_offset, transform.position.z);//x_position, y_position + z_offset, 0);
			} else {
				transform.localPosition = new Vector3(0, z_offset, transform.position.z);//x_position, y_position + z_offset, 0);
			}
		}

    //Throw this object from x starting position to y target
    //This method is a coroutine
    public virtual IEnumerator<float> Throw(Vector2 target, int arcDegrees) {
			beingThrown = true;
			//Ignore collisions between this object and the thrower for a short amount of time
			holderData.SetCollisionFlag(this, true);

			//Run-time variables
			float zRate = Mathf.Sqrt(9.8f*Vector2.Distance(GetPosition(), target));

			float arcRadians = arcDegrees*Mathf.PI/180;
			float factor = Mathf.Sin(arcRadians)/Mathf.Sin(Mathf.PI/4);

			//Set object to be in front or behind player depending on direction thrown
			Vector2 distance = target - (Vector2) GetPosition();
			//Possessed Objects have no "holder", and so the throw object could be called without holder being defined
			myRenderer.sortingOrder = holderData.GetSortingOrder() + (int) (-distance.y/Mathf.Abs(distance.y));
			Drop();

			shadow.addVelocity((zRate+(4.9f*Time.deltaTime))*Time.deltaTime*factor); //Arc is increased by a factor of 1/factor, multiplying distance by 1/factor
			distance = distance*(1f/factor); //By increasing distance by a multiple of inverse factor, the speed (and so distance) will be multiplied by factor
			//These two together cause distance to be the same; the distance to target, but it makes the speed and arc different
			shadow.PushDist(distance.normalized*distance.magnitude/(shadow.GetRigidbody().drag * (26f/15)), ForceMode2D.Impulse);

			//Wait for the object to hit the ground again
			while(shadow.GetHeight() != 0 || shadow.GetHeightVelocity() > 0) {
				UpdatePosition(shadow.GetHeight());
				//When object has come to half its distance and started falling back down, continuously change sortingOrder
				if(shadow.GetHeightVelocity() < 0 ) {
					myRenderer.sortingOrder = (int) (GetPosition().y*GlobalRegistry.SORTING_Y_MULTIPLIER());
				}
				yield return Timing.WaitForOneFrame;
			}
			//Object has reached target, so make vibration
			Vibration.Vibrator().MakeVibration((int) ((zRate)*FALL_VIBRATION_SIZE), (Vector2) GetPosition(), this);

			//Object may have crashed into other objects and hurt them, so empty the damagedList as well
			EmptyDamagedList();

			//Object is no longer attached to thrower, so let it be able to collide with thrower again
			holderData.SetCollisionFlag(this, false);
			beingThrown = false;
    }

		/* //IF NO ERRORS AROSE FROM THIS BEING REMOVED, GET RID OF THIS SLICE OF CODE; IT WAS NEVER USED
		public void UpdateZ(float z) {
			//Update Order in Layer
			myRenderer.sortingOrder = (int)(-GetPosition().y + z);
			//Update shadow depending on height from object to shadow
			shadow.UpdateSize();
		}*/

		/*
		public void reverseThrow() {
			toReverse = true;

			//NOTE: IF WE DECIDE THAT THE PLAYER WILL BE HIT BY THROWN OBJECTS THAT BOUNCE OFF WALLS AND HIT THEM,
			//ADD ENABLECOLLISION TO SHADOW'S REVERSETHROW METHOD

		}*/

		//Collision Variable for subclasses
		//protected virtual void OnCollisionEnter2D(Collision2D collision)
		protected virtual void OnTriggerEnter2D(Collider2D collision)
		{
			//Check if the object's concreteForm (parent) is also colliding with our object (on same z)
			Material touchedObj = collision.gameObject.GetComponent<Material>();
			//If the object has a shadow component, damage the touched object
			if(touchedObj) {
				//Set the objects to be able to touch concretely in case the shadow's OnTriggerEnter disabled collision
				touchedObj.GetShadow().SetCollisionFlag(shadow.GetSolidCollider(), false);
				if(GetShadow().GetCollider().IsTouching(touchedObj.GetShadow().GetCollider())) {
					//If it is, damage it by a certain amount. That shadow object will do the same to this object
					Attack(touchedObj, GetDamageAmount());
					//touchedObj.Damage(GetDamageAmount(), gameObject);
					//Disable collision between these two objects until collision exits
					//Physics2D.IgnoreCollision(myCollider, touchedObj.GetCollider(), true);
				} else {
					//If the objects' shadows are touching, but not the objects, they are offset by flying-height, and so should not collide
					touchedObj.GetShadow().SetCollisionFlag(shadow.GetSolidCollider(), true);
				}
			}
		}

		//protected void OnCollisionExit2D(Collision2D collision)
		protected void OnTriggerExit2D(Collider2D collision)
		{
			Material touchedObj = collision.gameObject.GetComponent<Material>();
			if(touchedObj) {
				//Physics2D.IgnoreCollision(myCollider, touchedObj.GetCollider(), false);
				touchedObj.GetShadow().SetCollisionFlag(shadow.GetSolidCollider(), false);
			}
		}

    //The "virtual" is important to show this method will be overriden
  	//This method is called whenever a vibration touches a moving object in the BlockingLayer collision layer
  	public virtual void FeelVibration (Vector2 sourcePosition) {}

    //Picks up this object and returns either an "Item" asset object for concreteItem objects
    //or null, telling the program the Use() function cannot be done on the item in the players' hand
    public virtual Item PickedUp(GameObject _holder, bool resetPositionFlag = true) {
      //Put the items together
      if(_holder != null) {
				holderData.Holder = _holder;
        Transform newPosition = _holder.transform;

				if(resetPositionFlag) {
					SetPosition(newPosition.position, 0); //0 for now!
		      //Place object on Object's layer so it looks like "holding" the object
					myRenderer.sortingLayerName = holderData.GetSortingLayerName(myRenderer.sortingLayerName);
					myRenderer.sortingOrder = holderData.GetSortingOrder()+1;
				}

        shadow.setFollow(newPosition);
				//Disable pickedUp object colliding or moving of its own accord
				DisableRigidbody();

      } else {
				shadow.setFollow(null);
      }
      //Return null so this object cannot be "Used" like an item; only thrown
      return null;
    }

		//Collision of Shadow is disabled
		public virtual void DisableRigidbody() {
			shadow.GetRigidbody().bodyType = RigidbodyType2D.Kinematic;
			shadow.DisableCollider();
			myCollider.enabled = false;
		}

		//Collision of Shadow is enabled
		public virtual void EnableRigidbody() {
			shadow.GetRigidbody().bodyType = RigidbodyType2D.Dynamic;
			shadow.EnableCollider();
			myCollider.enabled = true;
		}

    public void Drop() {
      holderData.Holder = null;
      shadow.setFollow(null);
			//Reactivate rigidbody and collision
			EnableRigidbody();
    }

    public virtual void PickUp(GameObject item) {
      item.GetComponent<Material>().PickedUp(gameObject);
    }

		public void UpdatePositionAtHand() {
			if(holderData.Holder != null) {
				//Create a variable to hold the size of this object divided by two so the exact center of this object will be held
				myCollider.enabled = true;
				Vector3 newPosition = holderData.GetHeldPosition(transform.position);
				if(newPosition != _lastPosition) {

					//Change position to fit hand
					float yPosition = newPosition.y;

					Vector3 mySize = myCollider.bounds.size/2;
					shadow.setHeight(-holderData.GetCollider().bounds.min.y + yPosition - mySize.y); //Add holder's size to this to make it accurate for walls
					shadow.setPosition(new Vector2(newPosition.x-myCollider.offset.x, holderData.GetCollider().bounds.min.y - GetShadow().GetOffset()+mySize.y));// - (holderData.GetCollider().bounds.size.y + (holderData.GetCollider().bounds.min.y - yPosition))));

					//Check that the item may have changed sorting order
					myRenderer.sortingOrder = holderData.GetSortingOrder();
				}
				_lastPosition = newPosition;
				myCollider.enabled = false;
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
					damaged.Damage(damageAmo*shadow.GetSize().x);
				}
			}
		}

		//Function that allows this object to take damage. Can take many forms. Only ever called by Attack(...)
		public virtual void Damage(float damageAmo) {
			//This is the command at the beginning of every damage method to make the damage less depending on object size
			damageAmo = damageAmo/(shadow.GetSize().x);
		}

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

    public GameObject GetHolder() {return holderData.Holder;}
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



		//HolderData Class used for all holder references
		public class HolderData {

			private Material holderScript;
			private GameObject holder;
			private SpriteRenderer holderRenderer;
			private Collider2D holderCollider;

			public HolderData() {
				holderScript = null;
				holder = null;
				holderRenderer = null;
				holderCollider = null;
			}

			//Property for holder GameObject
			public GameObject Holder {
				get{
					return holder;
				}
				set{
					holder = value;
					if(value) {
						holderScript = value.GetComponent<Material>();
						holderRenderer = value.GetComponent<SpriteRenderer>();
						holderCollider = value.GetComponent<Collider2D>();
					} else {
						holderScript = null;
						holderRenderer = null;
						holderCollider = null;
					}
				}
			}

			//Collision of Shadow is disabled
			public void SetCollisionFlag(Material userScript, bool flag) {
				if(userScript && holderScript) {
					//Object's Collider
					Physics2D.IgnoreCollision(userScript.GetCollider(), holderScript.GetCollider(), flag);
					//Shadow Regular Collider
					Physics2D.IgnoreCollision(userScript.GetShadow().GetCollider(), holderScript.GetShadow().GetCollider(), flag);
					//Shadow Solid Collider
					holderScript.GetShadow().SetCollisionFlag(userScript.GetShadow().GetSolidCollider(), flag);
				}
			}

			public int GetSortingOrder() {
				if(holderRenderer) {
					return holderRenderer.sortingOrder;
				} else {
					return 0;
				}
			}
			public string GetSortingLayerName(string oldLayerName) {
				if(holderRenderer) {
					return holderRenderer.sortingLayerName;
				} else {
					return oldLayerName;
				}
			}

			public Vector3 GetHeldPosition(Vector3 position) {
				if(holderScript) {
					return holderScript.GetHeldPosition(position);
				} else {
					return position;
				}
			}

			public Shadow GetShadow() {
				if(holderScript) {
					return holderScript.GetShadow();
				} else {
					//Yeah, you just caused an error. Don't ever call GetShadow on a holder
					//without first calling GetShadow and then checking the value of the method
					return null;
				}
			}

			public Collider2D GetCollider() {
				return holderCollider;
			}

		}
}
