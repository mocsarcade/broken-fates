using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {

    public int openingDir;
    //1 --> needs Bottom Door
    //2 --> needs Left Door
    //3 --> needs Top Door
    //4 --> needs Right door

    private RoomTemplate template;
    private int rand;
    private bool spawned = false;

    void Start () {
        template = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplate>();
        Invoke("Spawn", 0.5f);
    }
	
	void Spawn() {
        if(spawned == false)
        {
             if (openingDir == 1)
             {
                rand = Random.RandomRange(0, template.bottomRooms.Length);
                Instantiate(template.bottomRooms[rand], transform.position, template.bottomRooms[rand].transform.rotation);
             }
            else if (openingDir == 2)
            {
                rand = Random.RandomRange(0, template.leftRooms.Length);
                Instantiate(template.leftRooms[rand], transform.position, template.leftRooms[rand].transform.rotation);
            }
            else if (openingDir == 3)
            {
                rand = Random.RandomRange(0, template.topRooms.Length);
                Instantiate(template.topRooms[rand], transform.position, template.topRooms[rand].transform.rotation);
            }
            else if (openingDir == 4)
            {
                rand = Random.RandomRange(0, template.rightRooms.Length);
                Instantiate(template.rightRooms[rand], transform.position, template.rightRooms[rand].transform.rotation);
            }
            spawned = true;
        }
       
	}
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
           
           if(other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            {
                Instantiate(template.closedRoom[0], transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
           
           spawned = true;
        }
    }
}
