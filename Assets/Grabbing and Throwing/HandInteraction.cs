//- player can touch objects
//- player can squeeze trigger to grab objects
//- player can release trigger to drop objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteraction : MonoBehaviour {
    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    public float throwForce = 1.5f;

	//- To Swipe
	public float swipeSum;
	public float touchLast;
	public float touchCurrent;
	public float distance;
	public bool hasSwipedLeft;
	public bool hasSwipedRight;
	public ObjectMenuManager objectMenuManager;

	void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	void Update () {
        device = SteamVR_Controller.Input((int)trackedObj.index);

		if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Touchpad)) 
		{
			SteamVR_LoadLevel.Begin ("New Scene");
			touchLast = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
		}
			
		if (device.GetTouch (SteamVR_Controller.ButtonMask.Touchpad))
		{
			touchCurrent = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
			distance = touchCurrent - touchLast;
			touchLast = touchCurrent;
			swipeSum += distance;

			if (!hasSwipedRight) 
			{
				if (swipeSum>0.5f) 
				{
					swipeSum = 0;
					SwipeRight ();
					hasSwipedRight = true;
					hasSwipedLeft = false;
				}
			}
			if (!hasSwipedLeft) 
			{
				if (swipeSum < -0.5f) 
				{
					swipeSum = 0;
					SwipeLeft ();
					hasSwipedRight = false;
					hasSwipedLeft = true;
				}
			}
		}

		if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
		{
			swipeSum = 0;
			touchCurrent = 0;
			touchLast = 0;
			hasSwipedLeft = false;
			hasSwipedRight = false;
		}
		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
		{
			//- spawn object currently selected by menu
			SpawnObject();
		}
	}

	void SpawnObject() 
	{
		objectMenuManager.SpawnCurrentObject ();
	}

	void SwipeLeft() 
	{
		objectMenuManager.MenuLeft ();
		Debug.Log ("Swiped Left");
	}

	void SwipeRight()
	{
		objectMenuManager.MenuRight ();
		Debug.Log("Swiped Right");
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Throwable"))
        {
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                ThrowObject(other);
            }
            else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                GrabObject(other);
            }
        }
    }

    void ThrowObject (Collider coli) {

        coli.transform.SetParent(null);
        Rigidbody rigidbody = coli.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.velocity = device.velocity * throwForce;
        rigidbody.angularVelocity = device.angularVelocity;
        Debug.Log("You have thrown the object");

    }
    void GrabObject (Collider coli) {

        coli.transform.SetParent(gameObject.transform);
        coli.GetComponent<Rigidbody>().isKinematic = true;
        device.TriggerHapticPulse(2000);
        Debug.Log("You are touching down a trigger on an object");

    }
}
