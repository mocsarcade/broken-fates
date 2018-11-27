using System.Collections.Generic;
using UnityEngine;
using MEC;

public class SampleScene : Scene {

		//Variable inherited from Scene:
		//Dialogue[] dialogue;
		//GameObject[] target;
		//SpecialRoom containedRoom;

		void Awake() {
			dialogue = new Dialogue[1];
			dialogue[0] = new Dialogue("Zorlag", new string[] { "Huuuuuuuman! I am Haunting you!", "WhooooOOOh"});
		}

		public override IEnumerator<float> _RunScene() {
			SceneManager.instance.SetActivePlay(false);
			//Command 1: Run Dialogue
			yield return Timing.WaitUntilDone(Timing.RunCoroutine(
						SceneManager.instance._RunDialogue(dialogue[0])
						));
			//Command 2: Walk towards
			yield return Timing.WaitUntilDone(Timing.RunCoroutine(
						SceneManager.instance._MoveObjectTo(Player.GetPlayer(), target[0].transform.position)
						));

			SceneManager.instance.CloseDoors(containedRoom);
			SceneManager.instance.SetActivePlay(true);
			Debug.Log("Finishing!");
	}
}
