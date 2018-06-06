using UnityEngine;

// CreateAssetMenu allows you to construct a new item as you would a new C# script. Right click in assets, check under Inventory, then Item.
[CreateAssetMenu(fileName = "New AnimatorData", menuName = "Mechanics/AnimatorData")]
public class AnimatorData : ScriptableObject {

	public float X;
	public float Y;
	public bool Moving;

}
