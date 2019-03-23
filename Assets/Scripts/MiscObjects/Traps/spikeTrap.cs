using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class spikeTrap : Trap  {

    List<GameObject> collided;

    void Awake() {
        collided = new List<GameObject>();
    }

    protected override void Activate() {
        Debug.Log("Touched");
        foreach(GameObject damagedObj in collided) {
            Shadow touched = damagedObj.GetComponent<Shadow>();
            if(touched != null) {
                touched.GetParent().Damage(GameManager.instance.GetStamina()/2);
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        Debug.Log(collision.gameObject);
        collided.Add(collision.gameObject);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision);
        Debug.Log(collision.gameObject);
        collided.Remove(collision.gameObject);
    }



}

