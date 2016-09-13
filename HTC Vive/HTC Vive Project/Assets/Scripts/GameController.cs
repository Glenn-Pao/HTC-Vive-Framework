using UnityEngine;
using System.Collections;

//This is the script to track the game controller's button presses
public class GameController : MonoBehaviour 
{
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;               //grip button
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;    //trigger button

    private SteamVR_TrackedObject trackedObj;            //the tracked object

    //read only index of tracked object
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

	// Use this for initialization
	void Start () 
    {
        //track the object
	    trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        //find out what controls have been triggered
        //this is bound to change
        if(controller.GetPressDown(gripButton))
        {
            Debug.Log("Grip button was pressed");
        }

        if(!controller.GetPressDown(gripButton))
        {
            Debug.Log("Grip button was released");
        }

        if (controller.GetPressDown(triggerButton))
        {
            Debug.Log("Trigger button was pressed");
        }

        if (!controller.GetPressDown(triggerButton))
        {
            Debug.Log("Trigger button was released");
        }
	}
}
