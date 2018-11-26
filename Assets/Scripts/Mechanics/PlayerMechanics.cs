using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is the parent script for all other mechanics scripts
public class PlayerMechanics : MonoBehaviour { //<-To make game-mechanic-scripts inherit this script, replace "MonoBehaviour" with "Mechanics" in those scripts

	protected GameObject powerUser;
	protected MovingObject powerScript;
	public Sprite powerIcon;
	protected bool active;

	// Use this for initialization
	//Mechanics will likely be used by multiple objects; not just by the player. The initialize function affects that
	public virtual void Initialize (GameObject user) {
		powerUser = user;
		powerScript = user.GetComponent<MovingObject>();
	}

	//Method to be overwritten in all Mechanics classes that holds the code run when each power is activated
	//@return Boolean value telling program whether mechanic worked or if Stamina cost or barrier on the map stopped it
	public virtual bool Activate()
	{
		active = true;
		return false;
	}

	//Optional Method to be overwritten in all Mechanics classes that holds the code run when each power's key is released'
	public virtual void Release()
	{
		active = false;
	}

	//When power is swapped prematurely before power ends, this method is called to clean up the power
	public virtual void Deactivate(bool setMobility)
	{
		powerScript.SetMobility(setMobility);
	}

	//Optional Method to be overwritten in all Mechanics classes that holds the code run when each power's key is released'
	public bool isActive()
	{
		return active;
	}

	public Sprite GetIcon() {
		return powerIcon;
	}

	public virtual PlayerMechanics GetInstance() {
		return null;
	}

}
