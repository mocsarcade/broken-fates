using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Trap : MonoBehaviour
{
    protected int trapCooldown;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update() {
        if(trapCooldown > 0)
            trapCooldown--;
    }

	void OnTriggerStay2D(Collider2D collision) {
        //If trap CAN be activated
        if(trapCooldown <= 0) {
            //Check if it is also colliding with its shadow
            Shadow shadowScript = collision.GetComponent<Shadow>();
            if(shadowScript) {
                //Check dist to shadow
                if(shadowScript.GetHeight() < 0.5f) {
                    activate();
                }
            }/* else {
                activate();
            }*/
        }
    }

    public virtual void activate() {
        //Check if trap is can be activated
        Debug.Log("Activating!");
        trapCooldown = 200;
    }


}
