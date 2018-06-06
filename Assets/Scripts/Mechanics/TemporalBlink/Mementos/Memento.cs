using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memento : MonoBehaviour {

	public Material parent;
	public Vector3 position;
	public GameObject holder;

	// Use this for initialization
	public virtual void Initialize (Material _parent) {
		parent = _parent;
		position = _parent.transform.position;
		holder = _parent.getHolder();
	}

	public void setParent(Material _parent) {
		parent = _parent;
	}

	public virtual void Revert() {
		parent.useMemento(this);
	}
}
