using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	//The three slots for keys' mechanics. Initialized to TimeBlink, Dash and Stone
	public static Types.Mechanics[] keyMechanics = {Types.Mechanics.TimeBlink, Types.Mechanics.Dash, Types.Mechanics.Stone};

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
