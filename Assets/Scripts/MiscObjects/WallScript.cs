using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour {

	private SpriteRenderer myRenderer;
	private BoxCollider2D myCollider;
	public List<GameObject> childObjects = new List<GameObject>();

	//Initialize
	protected void Awake() {
		myRenderer = GetComponent<SpriteRenderer>();
		myCollider = GetComponent<BoxCollider2D>();
	}

	void Start () {
		myCollider.size = myRenderer.size;
		myRenderer.sortingOrder = (int) ((transform.position.y-myRenderer.bounds.extents.y) * GlobalRegistry.SORTING_Y_MULTIPLIER());

		/*
		foreach(GameObject child in childObjects) {
			Material childScript = child.gameObject.GetComponent<Material>();
			if(childScript) {
				childScript.PickedUp(gameObject);
			}
		}*/
	}

}
