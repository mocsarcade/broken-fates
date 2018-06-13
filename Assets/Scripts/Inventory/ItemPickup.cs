using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach ItemPickup component to an Item
public class ItemPickup : MonoBehaviour {

    public Item item;
    private const float PICKUP_RANGE = 2;
    // This is for writing to the screen whenever you pick up an item.
    public Dialogue dialogue;
    private DialogueManager dialogueWriter;

    // We will need to see where the item and player are in relation to each other
    // thisItem will take the transform value of the item it is attached to
    private Transform thisItem;
    private Transform player;

    public bool speaking;

    private void Awake()
    {
        thisItem = this.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        dialogueWriter = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (this.Distance() <= PICKUP_RANGE))
        {
            TriggerDialogue();
        }
    }

    public void PickUp()
    {
        // Add returns true if it can be picked up.
        bool wasPickedUp = Inventory.instance.PickUp(item);
        if (wasPickedUp)
        {
            Destroy(gameObject, 0);
        }
    }

    public float Distance()
    {
        return Vector3.Distance(thisItem.position, player.position);
    }

    public void TriggerDialogue()
    {
        if(speaking==false && dialogueWriter.getSpeaker() == null) {
          dialogueWriter.StartDialogue(dialogue, gameObject);
          speaking = true;
        } else if(dialogueWriter.getSpeaker() == gameObject) {
          //If a sentence has already been started,
          bool status = dialogueWriter.DisplayNextSentence();
          speaking = status;
          if(status == false) {
            PickUp();
          }
        }
    }

}
