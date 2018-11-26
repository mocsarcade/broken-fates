using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovingObjectMemento : Memento {

		protected Animator animator;
		protected AnimatorData animatorStateData;

		// Use this for initialization
		public override void Initialize (Material _parent) {
			MovingObject _parentObj = _parent as MovingObject;
			if(_parentObj != null) {
				animator = _parentObj.GetAnimator();
				animatorStateData = animator.GetBehaviour<AnimatorSaveScript>().GetData(animator);
				base.Initialize(_parentObj);
			} else {
				Debug.LogException(new Exception("MovingObject Memento expected parent to be a MovingObject!"), this);
			}
		}

		public override void Revert() {
			base.Revert();
			//Revert animator
			animator.GetBehaviour<AnimatorSaveScript>().RevertData(animatorStateData, animator);
		}
}
