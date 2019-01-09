using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is supposed to be attached to objects in game.
public class DialogueTrigger : MonoBehaviour {

    public Dialogue dialogue;
    private const float SPEAKING_DISTANCE = 2;

    private DialogueManager dialogueWriter;
    private Transform theSpeakerPosition;
    private Transform thePlayerPosition;
    public bool speaking;

    // On Awake, find transform's of Speaker and the Player.
    // Distance() calculates distances between the Speaker and Player.
    private void Awake()
    {
        dialogueWriter = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
        theSpeakerPosition = this.transform;
        thePlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public float Distance()
    {
        return Vector3.Distance(theSpeakerPosition.position, thePlayerPosition.position);
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (this.Distance() <= SPEAKING_DISTANCE))
        {
            Speak();
        }
    }*/

    public bool Speak()
    {
        if(speaking==false && dialogueWriter.GetSpeaker() == null) {
          dialogueWriter.StartDialogue(dialogue, gameObject);
          speaking = true;
          return true;
        } else if(dialogueWriter.GetSpeaker() == gameObject) {
          //If a sentence has already been started,
          bool status = dialogueWriter.DisplayNextSentence();
          speaking = status;
          return true;
        } else {
          return false;
        }
    }
}
