using UnityEngine;
using System.Collections;
using System;

public class Material : MonoBehaviour {

    public const int SHADOW_CHANGE_RATE = 5;
    public int weight;
    public int size;

    //Display variables
    public Transform myTransform;
    protected float z;
    //public SpriteRenderer renderer;
    public GameObject shadowObj;
    public Rigidbody2D rb2d;
    protected Shadow shadow;

    public int testDelay;

    void Awake() {
      myTransform = GetComponent<Transform>();
      rb2d = GetComponent<Rigidbody2D>();

      //REPLACE THIS WITH VECTOR OF APPROPRIATING SIZE BY SPRITERENDERER SIZE LATER
      Vector2 offset = Vector2.zero;
      //Initialize shadow to be a bigger blob depending on size of object and
      //give reference to transform so shadow will follow
      shadow = Instantiate(shadowObj, transform.position, Quaternion.identity).GetComponent<Shadow>();
      shadow.Initialize(myTransform, size, offset);

      testDelay=50;
    }

    void Update() {
      testDelay--;
      if(testDelay<0) {
        StartCoroutine(Throw(myTransform.position, new Vector2(6,0), 0.5f));
        testDelay=200;
      }
    }

    //Use method to instantly quaff items or use weapons
    public virtual void Use() {}

    public IEnumerator Throw(Vector2 start, Vector2 target, float strength) {
      Vector2 myPosition = (Vector2) myTransform.position;
      Vector2 nextPosition = myPosition;
      //Check that start is at least close to the object's actual position
      if(Vector2.Distance(myPosition, start) > 2) {
        Debug.LogException(new Exception("Throw method used TOO FAR from the actual object"), this);
      }
      //Deactivate Mobility and Shadow following
      shadow.Detach();
      //While less than halfway to target
      while(Vector2.Distance(myPosition + (Vector2.down*z),target) >= Vector2.Distance(start, target)/2)
      {
        //Get new Position
        //New position is decided by how powerful the throw is divided by the object's weight
        //A bottle would be weight 1, so a fast throw would have strength 2 or more
        nextPosition = Vector2.MoveTowards(myPosition, target, strength/weight);

        //Move Shadow
        shadow.setPosition(nextPosition);
        //Move Me
        //Moving an extra bit of upwards to simulate jumping/height
        rb2d.MovePosition(nextPosition + (Vector2.up*z));
        //Increase z
        z += strength/weight;

        //Update shadow depending on height from object to shadow
        if(z%SHADOW_CHANGE_RATE == 0) {
          shadow.UpdateSize((int) z);
        }

        //Yield until next frame
        yield return null;
        myPosition = shadow.getPosition();
      }
      //While more than halfway (not counting z) from target but still not at target
      while(Vector2.Distance(myPosition, target) > 0.05)
      {
        nextPosition = Vector2.MoveTowards(myPosition, target, strength/weight);
        //Move shadow
        shadow.setPosition(nextPosition);
        //Move Me
        //If shadow is halfway to target, stop the extra ascension and start
        //Making the artificial jump descend
        //But first make sure this descension isn't going too far
        if(z>0) {
          rb2d.MovePosition(nextPosition + (Vector2.down*z));
        }
        //Decrease z
        z -= strength/weight;

        //Update shadow depending on height from object to shadow
        if(z%SHADOW_CHANGE_RATE == 0) {
          shadow.UpdateSize((int) z);
        }

        //Yield until next frame
        yield return null;
        myPosition = shadow.getPosition();
      }

      //Now that we're done, reactivate Mobility
      shadow.Attach();
    }


}
