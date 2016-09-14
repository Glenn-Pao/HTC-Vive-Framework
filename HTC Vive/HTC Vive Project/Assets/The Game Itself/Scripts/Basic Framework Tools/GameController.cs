using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This is the script to track the game controller's button presses
public class GameController : MonoBehaviour 
{
    //private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;               //grip button
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;    //trigger button

    private SteamVR_TrackedObject trackedObj;            //the tracked object

    //read only index of tracked object
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();

    private InteractableItem closestItem;       //the closest item to the player
    private InteractableItem interactingItem;   //the item that is being interacted

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
		if(controller.GetPressDown(triggerButton))
        {
			Debug.Log ("triggerButton pressed down");

            float minDist = float.MaxValue;

            float distance;

            foreach(InteractableItem item in objectsHoveringOver)
            {
                //check the distance;
                distance = (item.transform.position - this.transform.position).sqrMagnitude;

                //if distance is less than the minimum distance
                if(distance < minDist)
                {
                    //redefine the minimum distance
                    minDist = distance;
                    closestItem = item;
                }
            }

            interactingItem = closestItem;

            if(interactingItem)
            {
                if(interactingItem.IsInteracting())
                {
                    interactingItem.EndInteraction(this);
                }
                interactingItem.BeginInteraction(this);
            }
        }

        //just to make sure there is something on hand and you intend to drop it
		if(controller.GetPressUp(triggerButton) && interactingItem != null)
        {
            interactingItem.EndInteraction(this);
        }
	}

    private void OnTriggerEnter(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if(collidedItem)
        {
            //add to the list of the hashset
            objectsHoveringOver.Add(collidedItem);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
        {
            //remove from the list of hashset
            objectsHoveringOver.Remove(collidedItem);
        }
    }
}
