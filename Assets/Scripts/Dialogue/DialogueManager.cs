using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class DialogueManager : MonoBehaviour {

    public Text nameText;
    public Text dialogueText;
    public Canvas SceneCanvas;
    public Animator animator;

    public GameObject speaker;

    // A Queue type works things in a first-in, first-out order.
    private Queue<string> sentences;

  	public static DialogueManager instance = null;

	void Awake()
	{
    //Make DialogueManager a Singleton
    if (instance == null)
      instance = this;
    else if (instance != this)
      Destroy(gameObject);
    //Hide the dialogue box at the beginning of the dialogue box's existence
		OpenDialogue (false);
	}

    private void Start()
    {
        sentences = new Queue<string>();
    }

    private void Update() {
        if(animator.GetBool("IsOpen")) {
          if(GlobalRegistry.CheckKey ("Next")) {
            DisplayNextSentence();
          }
        }
    }

    public void StartDialogue(Dialogue dialogue, GameObject _speaker)
    {
        // Make the player still (be nice and pay attention)
        Player.GetPlayer().SetMobility(false);
        Player.GetPlayer().allowInput(false);
        // Whenever IsOpen is set to true, the dialogue animation will move the dialoguebox to the screen.
        OpenDialogue (true);
        // The UI's nameText will be the name of the speaker, as designated in the inspector.
        nameText.text = dialogue.name;
        // Clear the queue so the dialogue is new
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();

        //Set the new speaking object
        speaker=_speaker;
    }

    //Displays next sentence in the current loaded sentences array
    //@return bool: Status of sentence. False if sentences has ended and true if sentence is continuing
    public bool DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return false;
        }
        string sentence = sentences.Dequeue();
        // If text is being displayed, it will stop!
        Timing.KillCoroutines("typingText");
        Timing.RunCoroutine(TypeSentence(sentence), "typingText");
        return true;
    }

    // The purpose of TypeSentence is so that we can display the dialogue slowly (character by character)
    private IEnumerator<float> TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            // This makes it wait
            yield return Timing.WaitForOneFrame;
        }
    }

    private void EndDialogue()
    {
        // Setting this boolean to false will trigger an animation, causing the dialogue box to move off screen.
        Player.GetPlayer().SetMobility(true);
        Player.GetPlayer().allowInput(true);
        OpenDialogue (false);
        speaker = null;
    }

  public GameObject GetSpeaker() {
    return speaker;
  }

	private void OpenDialogue(bool flag)
	{
    animator.SetBool("IsOpen", flag);
	}

  public bool isSpeaking() {
    return animator.GetBool("IsOpen");
  }
}
