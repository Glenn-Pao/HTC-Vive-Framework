using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//=============================================================================
//
// Purpose: Allow the player to be able to interact with the environment by picking up objects
// * Trigger click: grab any object that has a custom "Grabbable" tag applied
// * Trigger release: release the current grabbed object with relative force
// * Application Menu: reset the position of last grabbed object to controller
//
//=============================================================================
public class GameController : MonoBehaviour 
{
    public bool highlightGrabbableObject = true;
    public Color grabObjectHightlightColor;

    private SteamVR_TrackedObject trackedController;
    private SteamVR_Controller.Device device;

    HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();

    //private InteractableItem closestItem;       //the closest item to the player
    //private InteractableItem interactingItem;   //the item that is being interacted

    private Rigidbody controllerAttachPoint;
    private FixedJoint controllerAttachJoint;
    private GameObject canGrabObject;
    private Color[] canGrabObjectOriginalColors;
    private GameObject previousGrabbedObject;

	// Use this for initialization
	void Awake () 
    {
        //track the object
        trackedController = GetComponent<SteamVR_TrackedObject>();
	}

    void Start()
    {
        InitController();
    }

    //initialize the controller here
    void InitController()
    {
        //this is where the objects intended for grabbing will be attached to
        controllerAttachPoint = transform.GetChild(0).Find("tip").GetChild(0).GetComponent<Rigidbody>();

        //add a collider to the controller and set it as trigger type
        BoxCollider collider = this.gameObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(0.1f, 0.1f, 0.2f);
        collider.isTrigger = true;
    }

    void SnapCanGrabObjectToController(GameObject obj)
    {
        obj.transform.position = controllerAttachPoint.transform.position;

        controllerAttachJoint = obj.AddComponent<FixedJoint>();
        controllerAttachJoint.connectedBody = controllerAttachPoint;
        ToggleGrabbableObjectHighlight(false);
    }

    Rigidbody ReleaseGrabbedObjectFromController()
    {
        var jointGameObject = controllerAttachJoint.gameObject;
        var rigidbody = jointGameObject.GetComponent<Rigidbody>();
        Object.DestroyImmediate(controllerAttachJoint);
        controllerAttachJoint = null;

        return rigidbody;
    }

    void ThrowReleasedObject(Rigidbody rb)
    {
        var origin = trackedController.origin ? trackedController.origin : trackedController.transform.parent;
        if (origin != null)
        {
            rb.velocity = origin.TransformVector(device.velocity);
            rb.angularVelocity = origin.TransformVector(device.angularVelocity);
        }
        else
        {
            rb.velocity = device.velocity;
            rb.angularVelocity = device.angularVelocity;
        }

        rb.maxAngularVelocity = rb.angularVelocity.magnitude;
    }

    void RecallPreviousGrabbedObject()
    {
        if (previousGrabbedObject != null && device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            previousGrabbedObject.transform.position = controllerAttachPoint.transform.position;
            previousGrabbedObject.transform.rotation = controllerAttachPoint.transform.rotation;
            var rb = previousGrabbedObject.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.maxAngularVelocity = 0f;
        }
    }

    void UpdateGrabbableObjects()
    {
        if (canGrabObject != null)
        {
            if (controllerAttachJoint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                previousGrabbedObject = canGrabObject;
                SnapCanGrabObjectToController(canGrabObject);
            }
            else if (controllerAttachJoint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                Rigidbody releasedObjectRigidBody = ReleaseGrabbedObjectFromController();
                ThrowReleasedObject(releasedObjectRigidBody);
            }
        }
    }

    Renderer[] GetObjectRendererArray(GameObject obj)
    {
        return (obj.GetComponents<Renderer>().Length > 0 ? obj.GetComponents<Renderer>() : obj.GetComponentsInChildren<Renderer>());
    }

    Color[] BuildObjectColorArray(GameObject obj, Color defaultColor)
    {
        Renderer[] rendererArray = GetObjectRendererArray(obj);

        int length = rendererArray.Length;

        Color[] colors = new Color[length];
        for (int i = 0; i < length; i++)
        {
            colors[i] = defaultColor;
        }
        return colors;
    }

    Color[] StoreObjectOriginalColors(GameObject obj)
    {
        Renderer[] rendererArray = GetObjectRendererArray(obj);

        int length = rendererArray.Length;
        Color[] colors = new Color[length];

        for (int i = 0; i < length; i++)
        {
            var renderer = rendererArray[i];
            colors[i] = renderer.material.color;
        }

        return colors;
    }

    void ChangeObjectColor(GameObject obj, Color[] colors)
    {
        Renderer[] rendererArray = GetObjectRendererArray(obj);
        int i = 0;
        foreach (Renderer renderer in rendererArray)
        {
            renderer.material.color = colors[i];
            i++;
        }
    }

    void ToggleGrabbableObjectHighlight(bool highlightObject)
    {
        if (highlightGrabbableObject && canGrabObject != null)
        {
            if (highlightObject)
            {
                var colorArray = BuildObjectColorArray(canGrabObject, grabObjectHightlightColor);
                ChangeObjectColor(canGrabObject, colorArray);
            }
            else
            {
                ChangeObjectColor(canGrabObject, canGrabObjectOriginalColors);
            }
        }
    }

    void FixedUpdate()
    {
        device = SteamVR_Controller.Input((int)trackedController.index);

        RecallPreviousGrabbedObject();
        UpdateGrabbableObjects();
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Grabbable")
        {
            if (canGrabObject == null)
            {
                canGrabObjectOriginalColors = StoreObjectOriginalColors(collider.gameObject);
            }
            canGrabObject = collider.gameObject;
            ToggleGrabbableObjectHighlight(true);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Grabbable")
        {
            ToggleGrabbableObjectHighlight(false);
            canGrabObject = null;
        }
    }
}
