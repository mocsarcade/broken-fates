using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memento : MonoBehaviour {

	public Material parent;
	public Vector3 position;
	public Material holder;
	public Sprite sprite;

	public MementoData myData;

	// Use this for initialization
	public virtual void Initialize (Material _parent) {
		parent = _parent;
		position = _parent.transform.position;
		holder = _parent.getHolder();
		myData = _parent.getMementoData();
		sprite = _parent.getSprite();
	}

	public void setParent(Material _parent) {
		parent = _parent;
	}

	public Material getParent() {
		return parent;
	}

	public virtual void Revert() {
		if(parent != null) {
			if(parent.getExists() == false) {
				parent.Recreate();
			}
			parent.useMemento(this);
		} else {
			Debug.Log("Trying to bring back an object that was destroyed. It really was destroyed!");
		}
	}

	void OnDestroy() {
		if(parent != null) {
		if(parent.getExists() == false) {
			Debug.Log("Destroying parent object! Parent is a " + parent);
			Destroy(parent.gameObject);
		}
		if(myData != null) {
			Destroy(myData);
		}
		}
	}
//End Class
}
