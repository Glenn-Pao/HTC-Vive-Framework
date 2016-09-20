using UnityEngine;
using System.Collections;

//=============================================================================
//
// Purpose of this script: Allow the player to be able to move in the world space
//
// * Trackpad touch: Navigate through the world space based on finger position
// * Trackpad release: Slows player to a stop if moving
//
//=============================================================================
public class TouchpadMovement : MonoBehaviour 
{
    public TouchpadMovementTracking leftController;     //left controller
    public TouchpadMovementTracking rightController;    //right controller

    //to attach the camera rig here
    public Transform player;

    //the maximum walking speed of the player
    public float maxWalkSpeed = 2.0f;

    //the movement speed player currently is at
    private float movementSpeed = 0f;

    //the strafing speed player currently is at
    private float strafeSpeed = 0f;
    
    //deceleration or inertia so as to prevent jerking motion from sudden stop
    public float deceleration = 0.1f;

    //the player position in the world space
    private Vector3 playerPos;

    void Awake()
    {
        if (player == null)
        {
            player = FindObjectOfType<SteamVR_GameView>().transform;
        }
        
    }
	// Use this for initialization
	void Start () 
    {
        if (leftController == null)
        {
            Debug.Log("Left controller not initialized");
        }
        if (rightController == null)
        {
            Debug.Log("Right controller not initialized");
        }
	}

    //calculate the speed
    void CalculateSpeed(ref float speed, float inputValue)
    {
        if(inputValue != 0f)
        {
            speed = (maxWalkSpeed * inputValue);
        }
        else
        {
            Decelerate(ref speed);
        }
    }
    //move based on the headset's position
    void Move()
    {
        //Move forward or backwards
        var movement = player.transform.forward * Time.deltaTime * movementSpeed;

        //Move left or right
        var strafe = player.transform.right * Time.deltaTime * strafeSpeed;

        //fix the position of the y
        float fixY = transform.position.y;

        //add in the movement and strafe data into new position
        transform.position += (movement + strafe);

        //the new position
        transform.position = new Vector3(transform.position.x, fixY, transform.position.z);
    }

    void LeftControllerUpdate()
    {
        leftController.touchpad = leftController.device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
        CalculateSpeed(ref movementSpeed, leftController.touchpad.y);
        CalculateSpeed(ref strafeSpeed, leftController.touchpad.x);
    }
    void RightControllerUpdate()
    {
        rightController.touchpad = rightController.device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
        CalculateSpeed(ref movementSpeed, rightController.touchpad.y);
        CalculateSpeed(ref strafeSpeed, rightController.touchpad.x);
    }
    void FixedUpdate()
    {
        //ensure that it only updates when the controllers are around and the trackpad tracking is enabled
        if (leftController.gameObject.activeInHierarchy && leftController.TrackpadEnabled)
        {
            LeftControllerUpdate();
        }
        if(rightController.gameObject.activeInHierarchy && rightController.TrackpadEnabled)
        {
            RightControllerUpdate();
        }
        Move();
    }

    //pass in a reference of the speed here
    void Decelerate(ref float speed)
    {
        //if player is moving
        if (speed > 0)
        {
            speed -= Mathf.Lerp(deceleration, maxWalkSpeed, 0f);
        }
        //if player is moving in the negative direction
        else if (speed < 0)
        {
            speed += Mathf.Lerp(deceleration, -maxWalkSpeed, 0f);
        }
        //when player isn't moving
        else
        {
            speed = 0;
        }

        float deadzone = 0.1f;
        if (speed < deadzone && speed > -deadzone)
        {
            speed = 0;
        }
    }
}
