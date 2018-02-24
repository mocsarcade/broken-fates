using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public Text nameText;
    public Text dialogueText;

    // A Queue type works things in a first-in, first-out order.
    private Queue<string> sentences;

	void Awake()
	{
		HideDialogue ();
	}

    private void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // Make the player still (be nice and pay attention)
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().SetMobility(true);
        // Whenever IsOpen is set to true, the dialogue animation will move the dialoguebox to the screen.
		HideDialogue();
        // The UI's nameText will be the name of the speaker, as designated in the inspector.
        nameText.text = dialogue.name;
        // Clear the queue so the dialogue is new
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        // If text is being displayed, it will stop!
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    // The purpose of TypeSentence is so that we can display the dialogue slowly (character by character)
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            // This makes it wait
            yield return null;
        }
    }

    void EndDialogue()
    {
        // Setting this boolean to false will trigger an animation, causing the dialogue box to move off screen.
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().SetMobility(false);
        HideDialogue();
    }

	void HideDialogue()
	{
		GameObject.FindGameObjectWithTag("DialogueBox").GetComponent<Canvas>().enabled = 
			!GameObject.FindGameObjectWithTag("DialogueBox").GetComponent<Canvas>().enabled;
	}
}
