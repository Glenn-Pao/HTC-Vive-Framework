using UnityEngine;
using System.Collections;

//Touchpad Movement for HTC Vive framework

/// <summary>
/// This class is to provide the ability to move across the game world by sliding 
/// your finger over the touchpad. It should be applied to the [CameraRig] prefab
/// which should also contain a rigidbody and a box collider to ensure that
/// the user does not walk through collidable game objects.
/// </summary>

public class TouchpadMovement : MonoBehaviour 
{
    //toggle this to true to enable trackpad movement
    public bool TrackpadEnabled = true;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
