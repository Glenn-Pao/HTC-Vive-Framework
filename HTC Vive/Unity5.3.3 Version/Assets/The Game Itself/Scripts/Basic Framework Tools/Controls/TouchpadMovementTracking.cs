using UnityEngine;
using System.Collections;
//=============================================================================
//
// Purpose of this script: Tracks the input from controller
//
// * Trackpad touch: Navigate through the world space based on finger position
// * Trackpad release: Slows player to a stop if moving
//
//=============================================================================
public class TouchpadMovementTracking : MonoBehaviour 
{
    //toggle this to true to enable trackpad movement
    public bool TrackpadEnabled = true;

    //finger's position on touchpad
    public Vector2 touchpad;

    //tracked object, should be a controller
    SteamVR_TrackedObject trackedObj;

    //find out which device of the controller is used at that point in time
    public SteamVR_Controller.Device device;

	// Use this for initialization
	void Start () 
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        if(this.gameObject.activeInHierarchy)
        {
            device = SteamVR_Controller.Input((int)trackedObj.index);
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (this.gameObject.activeInHierarchy)
        {
            device = SteamVR_Controller.Input((int)trackedObj.index);
        }
	}
}
