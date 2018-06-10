using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemBox : MonoBehaviour {

	public Image itemIcon;
	public RectTransform myPosition;

	public int positionDegrees;
	public int degreesToMove;
	public int myItemIndex;

	// Use this for initialization
	void Awake () {
		myPosition = GetComponent<RectTransform>();
			itemIcon = myPosition.Find("Item").GetComponent<Image>();
	}

	public void Initialize(int _degrees, int _itemIndex) {
		positionDegrees = _degrees-1; degreesToMove=1;
		myItemIndex = _itemIndex;
		UpdateImage();
	}

	// Update is called once per frame
	void FixedUpdate () {
		if(Mathf.Abs(degreesToMove)>0) {
		//Change the current position of the box according to degreesToMove variable
		if(degreesToMove>0) {
			positionDegrees++;
			degreesToMove--;
		} else if(degreesToMove<0) {
			positionDegrees--;
			degreesToMove++;
		}
		//When the itemBox goes over 360 or under 0, it loops around to enter on the other side
		if(positionDegrees>=180) {
			positionDegrees -= 270;
		} else if(positionDegrees<-90) {
			positionDegrees += 270;
		}
		//Calculate new position
		myPosition.anchoredPosition = new Vector3(-45-85*Mathf.Cos(Mathf.Deg2Rad*positionDegrees),-45-85*Mathf.Sin(Mathf.Deg2Rad*positionDegrees), 0);
		}
	}

	public void addDegrees(int degrees) {
		degreesToMove += degrees;
	}

	public int getDegrees() {
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
}
