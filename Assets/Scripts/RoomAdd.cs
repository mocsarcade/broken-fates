using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomAdd : MonoBehaviour {
    private RoomTemplate template;
    // Use this for initialization
    void Start()
    {
        template = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplate>();
        template.rooms.Add(this.gameObject);

    }
}
