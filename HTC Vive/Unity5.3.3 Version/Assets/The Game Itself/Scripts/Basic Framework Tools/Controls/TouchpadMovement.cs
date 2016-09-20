using UnityEngine;
using System.Collections;

//you know what is needed for a little more immersion?
//Touchpad movement
//WARNING: DO NOT USE THIS IF YOU EXPERIENCE MOTION SICKNESS EASILY
public class TouchpadMovement : MonoBehaviour 
{
    //to attach the camera rig here
    public GameObject player;
    
    //toggle this to true to enable trackpad movement
    public bool TrackpadEnabled = true;

    //the maximum walking speed of the player
    public float maxWalkSpeed = 2.0f;

    //the movement speed player currently is at
    private float movementSpeed = 0f;

    //the strafing speed player currently is at
    private float strafeSpeed = 0f;
    
    //deceleration or inertia so as to prevent jerking motion from sudden stop
    public float deceleration = 0.1f;

    //finger's position on touchpad
    Vector2 touchpad;

    //the player position in the world space
    private Vector3 playerPos;

    //tracked object, should be a controller
    SteamVR_TrackedObject trackedObj;

    //find out which device of the controller is used at that point in time
    SteamVR_Controller.Device device;

	// Use this for initialization
	void Start () 
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
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
    void FixedUpdate()
    {
        //read the touchpad values here to determine the movement
        touchpad = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

        CalculateSpeed(ref movementSpeed, touchpad.y);
        CalculateSpeed(ref strafeSpeed, touchpad.x);
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
