using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is the parent script for all other mechanics scripts
public class Mechanics : MonoBehaviour { //<-To make game-mechanic-scripts inherit this script, replace "MonoBehaviour" with "Mechanics" in those scripts

	// Use this for initialization
	//Anything every mechanic script ever made in this game will need initialized at the beginning of the game will be here	
	void Start () {
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            QuickLife.control.Save();
        }
        if (Input.GetKeyUp(KeyCode.F9))
        {
            QuickLife.control.Load();
        }
    }

    // DrainStam will be made later, but it is a method all game-mechanics will use as all game-mechanics drain stamina
    public static void DrainStam (int amo) {
		//Code code code
	}
}
