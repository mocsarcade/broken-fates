using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {

    // Name of the person speaking (if any)
    public string name;

    [TextArea(3,10)]
    public string[] sentences;
}
