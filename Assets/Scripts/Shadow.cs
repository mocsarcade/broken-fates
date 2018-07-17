using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Shadow : MonoBehaviour {

	public const float NUM_SHADOW_IMAGES = 14f;

	protected Transform parentTransform;
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

					if(z_offset < 0) {
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
		Debug.Log(gameObject.name + " touching " + collision.gameObject.name);
		Shadow touchedShadow = collision.gameObject.GetComponent<Shadow>();
		//If the object has a shadow component, it could be damaged!
		if(touchedShadow) {
			//Check if the object's concreteForm (parent) is also colliding with our object (on same z)
			if(GetParentCollider().IsTouching(touchedShadow.GetParentCollider())) {
				Debug.Log("The objects are touching");
				//If it is, damage it by a certain amount. That shadow object will do the same to this object
				parentScript.Attack(touchedShadow.GetParent(), parentScript.GetDamageAmount());
				//Disable collision between these two objects until collision exits
				Physics2D.IgnoreCollision(GetParentCollider(), touchedShadow.GetParentCollider(), true);
			}
		}
		else if(parentScript.GetThrowState()) {
			//If it's not a shadow object, you've hit a wall, and you should bounce off of it
			reverseThrow();
		}
	}

	protected void reverseThrow() {
		rb2d.velocity = -rb2d.velocity;
	}

	//protected void OnCollisionExit2D(Collision2D collision)
	protected void OnTriggerExit2D(Collider2D collision)
	{
		Shadow touchedShadow = collision.gameObject.GetComponent<Shadow>();
		if(touchedShadow) {
			Physics2D.IgnoreCollision(GetParentCollider(), touchedShadow.GetParentCollider(), false);
		}
	}

}
