using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is the parent script for all other mechanics scripts
public class PlayerMechanics : MonoBehaviour { //<-To make game-mechanic-scripts inherit this script, replace "MonoBehaviour" with "Mechanics" in those scripts

	// Use this for initialization
	//Anything every mechanic script ever made in this game will need initialized at the beginning of the game will be here
	void Start () {
	}

	//Method to be overwritten in all Mechanics classes that holds the code run when each power is activated
	//@return Boolean value telling program whether mechanic worked or if Stamina cost or barrier on the map stopped it
	public virtual bool Activate()
	{
		return false;
	}

	//Optional Method to be overwritten in all Mechanics classes that holds the code run when each power's key is released'
	public virtual void Release()
	{
	}

}
