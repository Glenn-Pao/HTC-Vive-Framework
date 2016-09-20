using UnityEngine;
using System.Collections;

public class Fruit : MonoBehaviour {

	
	// Update is called once per frame
	void Update () 
    {
	    if(this.transform.position.y < 0f)
        {
            Destroy(this.gameObject);
        }
	}
}
