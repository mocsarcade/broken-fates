using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The superclass Memento is for all physical objects. The memento saves position, velocity and sprite
//Subclasses are used for intricate details, such as inventory space, animator direction or something of the sort

public class Memento : MonoBehaviour {

	//Object his Memento is made for
	public Material parent;

	//Private variables
	public Material holder;
	public Sprite sprite;

	public MementoData myData;

	//Position Variables
	public Vector3 position;
	public float z_offset;
	public float z_velocity;
	public Vector2 momentum;

	// Use this for initialization
	public virtual void Initialize (Material _parent) {
		parent = _parent;
		holder = _parent.GetHolder();
		sprite = _parent.GetSprite();
		myData = _parent.GetMementoData();
		position = _parent.GetPosition();
		z_offset = _parent.GetHeight();
		z_velocity = _parent.GetZVelocity();
		momentum = _parent.GetMomentum();
	}

	public void setParent(Material _parent) {
		parent = _parent;
	}

	public Material GetParent() {
		return parent;
	}

	public virtual void Revert() {
		if(parent != null) {
			if(parent.GetExists() == false) {
				parent.Recreate();
			}
			parent.useMemento(this);
		} else {
			Debug.Log("Trying to bring back an object that was destroyed. It really was destroyed!");
		}
	}

	void OnDestroy() {
		if(parent != null) {
		if(parent.GetExists() == false) {
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
