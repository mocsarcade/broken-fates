using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalRegistry : MonoBehaviour {

	private static GlobalRegistry instance = null;

	void Awake () {
		//Make GameManager a Singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	}

	public static void Reset() {
		instance._Reset();
	}
	private void _Reset() {
		wallRank = 0;
		numWalls = 0;
	}

	private float _INVERSE_DAMAGE_MULTIPLIER = 2f;
	public static float INVERSE_DAMAGE_MULTIPLIER() {
		//Call private method to get Inverse Damage Multiplier
		return instance.Get_Inverse_Damage_Multiplier();
	}
	//Private method used by public GET method
	private float Get_Inverse_Damage_Multiplier() {return _INVERSE_DAMAGE_MULTIPLIER;}


	private int _SORTING_Y_MULTIPLIER = -3;
	public static int SORTING_Y_MULTIPLIER() {
		//Call private method to get Multiplier for Y to get the object's sorting order
		return instance.Get_Sorting_Y_Multiplier();
	}
	//Private method used by public GET method
	private int Get_Sorting_Y_Multiplier() {return _SORTING_Y_MULTIPLIER;}


	private const float _PLAYER_REACH = 0.75f;
	public static float PLAYER_REACH() {
		//Call private method to get Reach
		return instance.Get_Player_Reach();
	}
	//Private method used by public GET method
	private float Get_Player_Reach() {return _PLAYER_REACH;}

	public const int _ROOM_WIDTH = 24;
	public static int ROOM_WIDTH() {
		//Call private method to get each room's width
		return instance.Get_Room_Width();
	}
	//Private method used by public GET method
	private int Get_Room_Width() {return _ROOM_WIDTH;}

	public const int _ROOM_HEIGHT = 18;
	public static int ROOM_HEIGHT() {
		//Call private method to get each room's height
		return instance.Get_Room_Height();
	}
	//Private method used by public GET method
	private int Get_Room_Height() {return _ROOM_HEIGHT;}

	private int wallRank = 0;
	private int numWalls = 0;
	public static int GetWallRank() {
		//Call private method to get this wall's rank
		return instance.Get_Wall_Rank();
	}
	//Private method used by public GET method
	private int Get_Wall_Rank() {
		wallRank += 1;
		numWalls = wallRank;
		return wallRank;
	}
	public static int GetWallNum() {
		return instance.Get_Wall_Num();
	}
	//Private method used by public GET method
	private int Get_Wall_Num() {
		return numWalls;
	}

  protected const float _SNEAK_FRACTION = 0.5f;
	public static float SNEAK_FRACTION() {
		//Call private method to get each room's height
		return instance.Sneak_Fraction();
	}
	//Private method used by public GET method
	private float Sneak_Fraction() {return _SNEAK_FRACTION;}

}
