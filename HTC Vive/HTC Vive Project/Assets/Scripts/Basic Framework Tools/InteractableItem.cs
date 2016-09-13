using UnityEngine;
using System.Collections;

//interactable items are probably a staple in a VR game.
//while this may not be used, it is a good idea to have this class
public class InteractableItem : MonoBehaviour 
{
    public Rigidbody rigidbody;         //the rigidbody of the object

    private bool currentlyInteracting;  //whether it is currently interacting with player

    private float velocityFactor = 1000.0f;
    private Vector3 posDelta;

    private float rotationFactor = 400f;
    private Quaternion rotationDelta;
    private float angle;
    private Vector3 axis;

    private GameController attachedController;

    private Transform interactionPoint;

	// Use this for initialization
	void Start () 
    {
        rigidbody = GetComponent<Rigidbody>();          //find the rigidbody
        interactionPoint = new GameObject().transform;  //the point of interaction
        velocityFactor /= rigidbody.mass;               //the new velocity factor based on mass
        rotationFactor /= rigidbody.mass;               //the new rotation factor based on mass
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    //Using this to do physics calculations
    void FixedUpate()
    {
        if (attachedController && currentlyInteracting)
        {
            posDelta = attachedController.transform.position - interactionPoint.position;
            this.rigidbody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

            rotationDelta = attachedController.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
            {
                angle -= 360;
            }


            if (angle < 5f)
            {
                this.rigidbody.angularVelocity = Vector3.zero;
            }
            else
            {
                this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
            }
        }
    }

    //attach the game object to the controller when this function is called
    public void BeginInteraction(GameController controller)
    {
        attachedController = controller;
        interactionPoint.position = controller.transform.position;
        interactionPoint.rotation = controller.transform.rotation;
        interactionPoint.SetParent(transform, true);

        currentlyInteracting = true;
    }

    public void EndInteraction(GameController controller)
    {
        //remove the game object from the controller
        if(controller == attachedController)
        {
            attachedController = null;
            currentlyInteracting = false;
        }
    }

    public bool IsInteracting()
    {
        return currentlyInteracting;
    }
}
