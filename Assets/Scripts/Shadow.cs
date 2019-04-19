using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Shadow : MonoBehaviour {

	public const float NUM_SHADOW_IMAGES = 14f;

	protected Transform parentTransform;
	protected SpriteRenderer parentRenderer;
	protected Rigidbody2D rb2d;
	protected CapsuleCollider2D myCollider;
	protected BoxCollider2D solidCollider;
	protected Animator animator;
	public Material parentScript;
	public float size;
	public bool following = true;
	public float y_offset;
	public float z_offset;
	public float z_velocity;

	void OnEnable() {
		rb2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		myCollider = GetComponent<CapsuleCollider2D>();
		solidCollider = GetComponent<BoxCollider2D>();
	}

/*
	void FixedUpdate() {
		//If Z is high enough, Decrease Z by fall amount and update Position
		if(z_offset > 0 || z_velocity > 0) {
			addHeight(z_velocity);

			//addVelocity(-9.8f * Time.deltaTime*0.04f);
			addVelocity(-9.8f * Time.deltaTime * Time.deltaTime);

			if(z_offset < 0) {
				z_offset = 0;
				z_velocity = 0;
			}
			parentScript.UpdatePosition(z_offset);
		}
	}*/

	void Update() {
		if(z_offset < 0.1f) {
			parentRenderer.sortingOrder = (int) (-GetPosition().y*3);
		}
	}

	protected IEnumerator<float> Gravity() {
		float drag = rb2d.drag;
		while(1==1) {
			//Bring object towards floor unless being held in the air
			if(parentScript.GetHolder() == null) {
				//If Z is high enough, Decrease Z by fall amount and update Position
				if(z_offset > 0 || z_velocity > 0) {
					rb2d.drag = 0;
					addHeight(z_velocity);
					addVelocity(-9.8f * Time.deltaTime * Time.deltaTime * (11f/15));

					if(z_offset <= 0) {
						z_offset = 0;
						z_velocity = 0;
						rb2d.drag = drag;
					}
					parentScript.UpdatePosition(z_offset);
				}
			}
			yield return Timing.WaitForOneFrame;
		}
	}

	public void Initialize(Transform _parent, int weight, Collider2D parentSize) {
		parentTransform = _parent;
		parentScript = _parent.gameObject.GetComponent<Material>();
		parentRenderer = _parent.gameObject.GetComponent<SpriteRenderer>();

		//Set shadow and Colliders' size
		size = parentSize.bounds.size.x*6;
		animator.SetFloat("Size", (size)/NUM_SHADOW_IMAGES);
		myCollider.size = new Vector2((size)/7f, (size)*2f/21f);

		solidCollider.size = new Vector2((size)*2f/21f, (size)*2f*2/(21f*3));

		SpriteRenderer shadowRenderer = GetComponent<SpriteRenderer>();
		y_offset = -(parentSize.bounds.min.y - shadowRenderer.bounds.extents.y + shadowRenderer.bounds.extents.y - parentTransform.position.y);
		//Set Initial Position
		transform.position = parentTransform.position + Vector3.down * y_offset;

		//Set the rigidbody's mass
		rb2d.mass = size*weight;

		//NOTE: LATER, SHOULD ICE, SOAP OR SLIPPERY FLOORS BE USED, ADD A SECTION HERE TO SET LINEAR DRAG

		//Start the gravity
		Timing.RunCoroutine(Gravity().CancelWith(_parent.gameObject), Segment.FixedUpdate);
	}

	public void setVisible(bool visible) {
		GetComponent<SpriteRenderer>().enabled = visible;
	}

	public void UpdateSize () {
		float dist = z_offset;
		if(dist>size)
			dist=size;
		else if(dist<0)
			dist=0;
		animator.SetFloat("Size", (size-dist)/NUM_SHADOW_IMAGES);
		parentScript.UpdatePosition(z_offset);
	}

	public void Push (Vector2 force, ForceMode2D moveType) {
		rb2d.AddForce(force*rb2d.mass, moveType);
	}

	//public void PushDist (Vector2 force, ForceMode2D moveType) {
	public void PushDist (Vector2 force, ForceMode2D moveType) {
		rb2d.AddForce(force*rb2d.mass*rb2d.drag, moveType);
	}

	//Used to call shadow to player when throwing begins (making sure nothing sleazy happens)
	public void setPosition (Vector2 newPosition) {
		//rb2d.MovePosition(newPosition + offset);
		transform.position = (Vector3) (newPosition);// + Vector2.down * y_offset);
	}

	public void addHeight (float z) {
		z_offset += z;
		UpdateSize();
	}

	public void addVelocity (float velo) {
		if(z_offset > 0 || velo > 0) {
			z_velocity += velo;
		}
	}

	//Used to call shadow to player when throwing begins (making sure nothing sleazy happens)
	public void setObjectPosition (Vector2 newPosition) {
		//rb2d.MovePosition(newPosition + offset);
		transform.position = (Vector3) (newPosition + Vector2.down * y_offset);
	}

	public void setFollow (Transform newTransformParent) {
		transform.parent = newTransformParent;
	}

	public Vector2 GetSize() {
		return myCollider.size;
	}

	public Vector2 GetPosition () {
		return (Vector2) transform.position;// - Vector2.down * y_offset;
	}

	public Vector2 GetObjectPosition () {
		return (Vector2) transform.position - Vector2.down * (y_offset + z_offset);
	}

	public Collider2D GetCollider () {return myCollider;}
	public Collider2D GetParentCollider () {return parentScript.GetCollider();}

	public void DisableCollider () {
		myCollider.enabled = false;
		solidCollider.enabled = false;
	}
	public void EnableCollider () {
		myCollider.enabled = true;
		solidCollider.enabled = true;
	}

	public Material GetParent () {return parentScript;}
	public Rigidbody2D GetRigidbody () {return rb2d;}
	public Collider2D GetSolidCollider () {return solidCollider;}
	public Vector2 GetMomentum () {return rb2d.velocity;}
	public void setMomentum (Vector2 momentum) {
		rb2d.velocity = momentum;
	}

	public float GetOffset () {return y_offset;}
	public float GetHeight () {return z_offset;}
	public float GetHeightVelocity () {return z_velocity;}
	public void setHeight (float z, float velo) {
		z_offset = z;
		z_velocity = velo;
		UpdateSize();
	}
	public void setHeight (float z) {
		z_offset = z;
		UpdateSize();
	}
	public float GetSpeed () {return rb2d.velocity.magnitude;}

	//When this thrown object bounces
	//protected void OnCollisionEnter2D(Collision2D collision)
	protected void OnTriggerEnter2D(Collider2D collision)
	{
		//If the object has a shadow component, it could be damaged!
		if(collision.gameObject.layer == LayerMask.NameToLayer("All Shadows"))
		{
			Shadow touchedShadow = collision.gameObject.GetComponent<Shadow>();
			if(touchedShadow) {
				//Set the objects to be able to touch concretely in case the object's OnTriggerEnter disabled collision
				touchedShadow.SetCollisionFlag(solidCollider, false);
				//Check if the object's concreteForm (parent) is also colliding with our object (on same z)
				if(GetParentCollider().IsTouching(touchedShadow.GetParentCollider())) {
					//If it is, damage it by a certain amount. That shadow object will do the same to this object
					parentScript.Attack(touchedShadow.GetParent(), parentScript.GetDamageAmount());
				} else {
					//If the objects' shadows are touching, but not the objects, they are offset by flying-height, and so should not collide
					touchedShadow.SetCollisionFlag(solidCollider, true);
				}
			}
		}
		//If the object is a Vibration, do nothing!
		else if(collision.gameObject.layer == LayerMask.NameToLayer("Vibration"))
		{
			//Nothing!
		}
		//If it's neither a shadow object nor a vibration, you've hit a wall
		else {
			//Check if you should bounce off of it
			if(parentScript.GetThrowState()) {
				ReverseThrow();
				//MakeVibration(collision);
			}
		}
	}

	protected void ReverseThrow() {
		//Reverse Direction
		rb2d.velocity = -rb2d.velocity;
	}

	protected void MakeVibration(Collider2D collision) {
		//Make sure the object is touching the object the shadow touched. If not, the object is too high and is over the wall
		//This is too far to see or appreciate the vibration, so don't show it
		//Debug.Log(parentTransform.gameObject.GetComponent<Collider2D>().Distance(collision).isOverlapped + " and distance is " + parentTransform.gameObject.GetComponent<Collider2D>().Distance(collision).distance);
		if(parentTransform.gameObject.GetComponent<Collider2D>().Distance(collision).isOverlapped) {
			//Make Vibration
			Vibration.Vibrator().MakeVibration((int) (rb2d.velocity.magnitude*100), (Vector2) parentTransform.position + rb2d.velocity*Time.deltaTime, this.gameObject, collision);
		}
	}

	public void FeelVibration (Vector2 sourcePosition) {
		parentScript.FeelVibration(sourcePosition);
	}

	//protected void OnCollisionExit2D(Collision2D collision)
	protected void OnTriggerExit2D(Collider2D collision)
	{
		Shadow touchedShadow = collision.gameObject.GetComponent<Shadow>();
		if(touchedShadow) {
			touchedShadow.SetCollisionFlag(solidCollider, false);
		}
	}

	public void SetCollisionFlag(Collider2D toDisable, bool flag) {
		Physics2D.IgnoreCollision(solidCollider, toDisable, flag);
	}

}
