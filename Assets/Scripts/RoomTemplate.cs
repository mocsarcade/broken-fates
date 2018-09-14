using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplate : MonoBehaviour {
    public GameObject[] topRooms;
    public GameObject[] bottomRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;
    public GameObject[] closedRoom;

    public List<GameObject> rooms;

    public float waitTime;

    private bool exitSpawned = false;

    public GameObject xit;   
	
	// Update is called once per frame
	void Update () {
        if(waitTime >= 300 && exitSpawned == false)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if(i == rooms.Count-1)
                {
                    Instantiate(xit, rooms[i].transform.position, Quaternion.identity);
                    exitSpawned = true;
                }
            }
        }
		else
        {
            waitTime = Time.frameCount;
        }
	}
}
