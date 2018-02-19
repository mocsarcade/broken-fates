using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public Animator animator;

    public Text nameText;
    public Text dialogueText;

    // A Queue type works things in a first-in, first-out order.
    private Queue<string> sentences;

    private void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // Whenever IsOpen is set to true, the dialogue animation will move the dialoguebox to the screen.
        animator.SetBool("IsOpen", true);
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
        dialogueText.text = sentence;
    }

    void EndDialogue()
    {
        // Setting this boolean to false will trigger an animation, causing the dialogue box to move off screen.
        animator.SetBool("IsOpen", false);
    }
}
