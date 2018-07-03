using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemBox : MonoBehaviour {
	public const int INVENTORY_SCROLL_SPEED = 2;

	public Image itemIcon;
	public RectTransform myPosition;

	public float positionDegrees;
	public float degreesToMove;
	public int myItemIndex;
	public int targetSize;
	public bool toDestroy;

	// Use this for initialization
	void Awake () {
		myPosition = GetComponent<RectTransform>();
		itemIcon = myPosition.Find("Item").GetComponent<Image>();
		targetSize = 50;
	}

	public void Initialize(float _degrees, int _itemIndex) {
		positionDegrees = _degrees-1; degreesToMove=1;
		myItemIndex = _itemIndex;
		UpdateImage();
	}

	void FixedUpdate () {
		//Resize Box
		if(Mathf.Abs(myPosition.sizeDelta.x-targetSize) > 1) {
  		if(myPosition.sizeDelta.x > targetSize) {
	  		myPosition.sizeDelta = new Vector2(myPosition.sizeDelta.x-1, myPosition.sizeDelta.y-1);
	  	} else {
	  		myPosition.sizeDelta = new Vector2(myPosition.sizeDelta.x+1, myPosition.sizeDelta.y+1);
	  	}
			if(myPosition.sizeDelta.x <= 0 && toDestroy == true) {
		    Destroy(gameObject);
			}
		}

		//Move Box
		if(Mathf.Abs(degreesToMove)>0) {
		//Change the current position of the box according to degreesToMove variable multiple times (for speed)
		for(int i=0; i<INVENTORY_SCROLL_SPEED; i++)
		{
			if(Mathf.Abs(degreesToMove)<1) {
				positionDegrees -= degreesToMove;
				degreesToMove = 0f;
			} else if(degreesToMove>0) {
				positionDegrees++;
				degreesToMove--;
			} else if(degreesToMove<0) {
				positionDegrees--;
				degreesToMove++;
			}
		}
		//When the itemBox goes over 360 or under 0, it loops around to enter on the other side
		if(positionDegrees>=180) {
			positionDegrees -= 270;
		} else if(positionDegrees<-90) {
			positionDegrees += 270;
		}
		//Calculate new position
		myPosition.anchoredPosition = new Vector3(-55-100*Mathf.Cos(Mathf.Deg2Rad*positionDegrees),-55-100*Mathf.Sin(Mathf.Deg2Rad*positionDegrees), 0);
		}
	}

	public void setToDestroy() {
		toDestroy = true;
	}

	public void resizeUIImage(int newSize) {
		targetSize = newSize;
	}

	public void addDegrees(float degrees) {
		degreesToMove += degrees;
	}

	public float getDegrees() {
		return positionDegrees+degreesToMove;
	}

	public void UpdateImage() {
		Item newItem = Inventory.instance.GetItem(myItemIndex);
		if(newItem != null) {
			itemIcon.sprite = newItem.icon;
			itemIcon.color = Color.white;
		} else {
			itemIcon.sprite = null;
			itemIcon.color = Color.clear;
		}
	}

	public void UpdateHandIndex(int offset) {
		myItemIndex += offset;
	}
}
