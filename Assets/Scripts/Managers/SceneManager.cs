using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

using SpecialDungeonRooms;

public class SceneManager : MonoBehaviour {

  	public static SceneManager instance = null;

    private float EPSILON = 0.5f;

    // Use this for initialization
  	void Awake () {
  		//Make Manager a Singleton
  		if (instance == null)
  			instance = this;
  		else if (instance != this)
  			Destroy(gameObject);
  	}

    public IEnumerator<float> _RunDialogue(Dialogue sentence) {
      DialogueManager.instance.StartDialogue(sentence, null);
      while(DialogueManager.instance.isSpeaking()) {
  			yield return Timing.WaitForOneFrame;
      }
    }

    public void SetActivePlay(bool flag) {
      GameManager.instance.SetActivePlay(flag);
    }

    public void _SetAnimation() {

    }

    public void CloseDoors(SpecialRoom room) {
      room.CloseRoom();
    	Timing.RunCoroutine(GameManager.CameraShake(2).CancelWith(Camera.main.gameObject));
    }

    /*
    * Moves a MovingObject script to a certain position
    */
    public IEnumerator<float> _MoveObjectTo(MovingObject source, Vector3 target) {
      while(Vector2.Distance((Vector2) source.transform.position, (Vector2) target) > EPSILON) {

        Vector2 movementVector = new Vector2(
          target.x - source.transform.position.x,
          target.y - source.transform.position.y);
        movementVector.Normalize();

        source.Move(movementVector);
        source.UpdateAnimator(movementVector);
    		yield return Timing.WaitForOneFrame;
      }
    }

    /*
    * Overloaded: Moves a MovingObject script to a certain position
    */
    public IEnumerator<float> _MoveObjectTo(MovingObject source, Vector2 target) {
      while(Vector2.Distance((Vector2) source.transform.position, target) > EPSILON) {

        Vector2 movementVector = new Vector2(
          target.x - source.transform.position.x,
          target.y - source.transform.position.y);
        movementVector.Normalize();

        source.Move(movementVector);
        source.UpdateAnimator(movementVector);
    		yield return Timing.WaitForOneFrame;
      }
    }

    /*
    * Overloaded: Moves a MovingObject script to a certain position
    */
    public IEnumerator<float> _MoveObjectTo(MovingObject source, GameObject target) {
      while(Vector2.Distance((Vector2) source.transform.position, (Vector2) target.transform.position) > EPSILON) {

        Vector2 movementVector = new Vector2(
          target.transform.position.x - source.transform.position.x,
          target.transform.position.y - source.transform.position.y);
        movementVector.Normalize();

        source.Move(movementVector);
        source.UpdateAnimator(movementVector);
    		yield return Timing.WaitForOneFrame;
      }
    }

}
