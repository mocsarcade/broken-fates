using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimScript : AnimatorSaveScript {

	public override AnimatorData GetData(Animator myAnimator) {
		AnimatorData currentStateData = Object.Instantiate((AnimatorData) GameManager.instance.GetDataReference(GameManager.DataType.t_AnimatorData));
		currentStateData.X = myAnimator.GetFloat("MoveX");
		currentStateData.Y = myAnimator.GetFloat("MoveY");
		currentStateData.Moving = false;
		return currentStateData;
	}

	public override void RevertData(AnimatorData currentStateData, Animator myAnimator) {
		myAnimator.SetFloat("MoveX", currentStateData.X);
		myAnimator.SetFloat("MoveY", currentStateData.Y);
		myAnimator.SetBool("Moving", currentStateData.Moving);
	}
}
