using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loader : MonoBehaviour {

	public GameObject gameManager;
	public Text subNameText;
	public Text subDialogueText;

	// Use this for initialization
	void Awake () {
		if (GameManager.instance == null) {
			GameObject gameController = Instantiate (gameManager);
			//gameController.GetComponent<DialogueManager>().nameText = subNameText;
			//gameController.GetComponent<DialogueManager>().dialogueText = subDialogueText;
		}
	}
}
