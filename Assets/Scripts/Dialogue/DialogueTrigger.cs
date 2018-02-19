using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is supposed to be attached to objects in game. 
public class DialogueTrigger : MonoBehaviour {

    public Dialogue dialogue;
    private float speakingDistance = 2;

    private Transform theSpeakerPosition;
    private Transform thePlayerPosition;

    // On Awake, find transform's of Speaker and the Player.
    // Distance() calculates distances between the Speaker and Player.
    private void Awake()
    {
        theSpeakerPosition = this.transform;
        thePlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public float Distance()
    {
        return Vector3.Distance(theSpeakerPosition.position, thePlayerPosition.position);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (this.Distance() <= speakingDistance))
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
