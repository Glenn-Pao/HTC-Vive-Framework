using UnityEngine;
using System.Collections;
//=============================================================================
//
// Purpose of this script: Allow the player to be able to teleport based on where he is facing
//
// * Trackpad press down: project laser pointer from Touch Controller
// * Trackpad release: teleport player with blink to laser point destination
//
//=============================================================================
public class Teleportation : MonoBehaviour 
{
    public Color pointerColor;                                          //color of pointer
    public float pointerThickness = 0.005f;                             //thickness of line
    public float pointerLength = 100f;                                  //how long the pointer's raycast will extend
    public bool showPointerTip = true;                                  //whether there is a tip to show
    public bool teleportWithPointer = true;                             //if false, it doesn't allow teleportation
    public float blinkTransitionTiming = 0.6f;                          //how long it takes to transition during teleportation

    private Vector3 pointerTipScale = new Vector3(0.05f, 0.05f, 0.05f); //the tip's scale
    private float pointerContactDist = 0f;                              //the distance at which the pointer contacts the floor
    private Transform pointerContactTarget = null;                      //any target in sight?

    private Transform HeadsetCameraRig;                                 //the camera's position
    private float HeadsetInitialYPosition;                              //camera's initial Y position
    private Vector3 TeleportLocation;                                   //the teleport location

    private SteamVR_TrackedObject trackedController;                    //which controller it is
    private SteamVR_Controller.Device device { get { return SteamVR_Controller.Input((int)trackedController.index); } }                           //the type of device

    private GameObject pointerHolder;
    private GameObject pointer;
    private GameObject pointerTip;

    void Awake()
    {
        trackedController = GetComponent<SteamVR_TrackedObject>();
    }

	// Use this for initialization
	void Start () 
    {
        InitPointer();
        InitHeadsetReferencePoint();
	}

    //the initialization of pointer functions
    void InitPointer()
    {
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", pointerColor);

        //create the pointer's initial position
        pointerHolder = new GameObject();
        pointerHolder.transform.parent = this.transform;
        pointerHolder.transform.localPosition = Vector3.zero;

        //create the pointer line
        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.parent = pointerHolder.transform;
        pointer.GetComponent<MeshRenderer>().material = newMaterial;

        //along with a collider that is kinematic and collider functions on trigger
        pointer.GetComponent<BoxCollider>().isTrigger = true;
        pointer.AddComponent<Rigidbody>().isKinematic = true;
        pointer.layer = 2;

        //create the pointer tip
        pointerTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointerTip.transform.parent = pointerHolder.transform;
        pointerTip.GetComponent<MeshRenderer>().material = newMaterial;
        pointerTip.transform.localScale = pointerTipScale;

        //along with a collider that is kinematic and collider functions on trigger
        pointerTip.GetComponent<SphereCollider>().isTrigger = true;
        pointerTip.AddComponent<Rigidbody>().isKinematic = true;
        pointerTip.layer = 2;

        SetPointerTransform(pointerLength, pointerThickness);
        TogglePointer(false);
    }

    //this is needed to find the reference point for the pointer to work
    void InitHeadsetReferencePoint()
    {
        
        #if(UNITY_5_3)
                Transform eyeCamera = GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
                // The referece point for the camera is two levels up from the SteamVR_Camera
                HeadsetCameraRig = eyeCamera.parent.parent;
                HeadsetInitialYPosition = HeadsetCameraRig.transform.position.y;
        #endif
        #if(UNITY_5_4)
                Transform eyeCamera = GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
                // The referece point for the camera is two levels up from the SteamVR_Camera
                HeadsetCameraRig = eyeCamera.parent;
                HeadsetInitialYPosition = HeadsetCameraRig.transform.position.y;
        #endif
    }

    void SetPointerTransform(float setLength, float setThickness)
    {   
        //a weird glitch popped out, so there is a decimal offset
        float beamPosition = setLength / (2 + 0.00001f);

        pointer.transform.localScale = new Vector3(setLength, setThickness, setThickness);
        pointer.transform.localPosition = new Vector3(beamPosition, 0f, 0f);
        pointerTip.transform.localPosition = new Vector3(setLength - (pointerTip.transform.localScale.x / 2), 0f, 0f);

        TeleportLocation = pointerTip.transform.position;
    }
    float GetPointerBeamLength(bool hasRayHit, RaycastHit collidedWith)
    {
        float actualLength = pointerLength;

        //reset if beam not hitting or hitting new target
        if (!hasRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
        {
            pointerContactDist = 0f;
            pointerContactTarget = null;
        }

        //check if beam has hit a new target
        if (hasRayHit)
        {
            if (collidedWith.distance <= 0)
            {

            }
            pointerContactDist = collidedWith.distance;
            pointerContactTarget = collidedWith.transform;
        }

        //adjust beam length if something is blocking it
        if (hasRayHit && pointerContactDist < pointerLength)
        {
            actualLength = pointerContactDist;
        }

        return actualLength; ;
    }

    void TogglePointer(bool state)
    {
        pointer.gameObject.SetActive(state);
        bool tipState = (showPointerTip ? state : false);
        pointerTip.gameObject.SetActive(tipState);
    }

    void Teleport()
    {
        SteamVR_Fade.Start(Color.black, 0);
        SteamVR_Fade.Start(Color.clear, blinkTransitionTiming);
        HeadsetCameraRig.position = new Vector3(TeleportLocation.x, HeadsetInitialYPosition, TeleportLocation.z);
    }

    void UpdatePointer()
    {
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            TogglePointer(true);
        }

        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (pointerContactTarget != null && teleportWithPointer)
            {
                Teleport();
            }
            TogglePointer(false);
        }

        if (pointer.gameObject.activeSelf)
        {
            Ray pointerRaycast = new Ray(transform.position, transform.forward);
            RaycastHit pointerCollidedWith;
            bool rayHit = Physics.Raycast(pointerRaycast, out pointerCollidedWith);
            float pointerBeamLength = GetPointerBeamLength(rayHit, pointerCollidedWith);
            SetPointerTransform(pointerBeamLength, pointerThickness);
        }
    }
	// Update is called once per frame
	void Update () 
    {
        UpdatePointer();
	}
}
