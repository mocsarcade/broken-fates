using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Some items will have an itemScript which designates what the object does when used.
//Items like rocks will have no ItemScript and so will do nothing when used.
public class ItemScript : MonoBehaviour {

	public virtual bool Use () {
		//An item with this general script will not be destroyed upon use
		return false;
	}
}
