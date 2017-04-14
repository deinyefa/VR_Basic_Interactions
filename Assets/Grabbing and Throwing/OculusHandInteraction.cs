//- player can touch objects
//- player can squeeze trigger to grab objects
//- player can release trigger to drop objects

//- for oculus, Fixed Timestep (in time manager) should be set to 0.111111111111...

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusHandInteraction : MonoBehaviour {
    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    public float throwForce = 1.5f;

	private OVRInput.Controller thisController;
	public bool leftHand; 									//- if true, this is the left hand controller

	//- To Swipe
	public float swipeSum;
	public float touchLast;
	public float touchCurrent;
	public float distance;
	public bool hasSwipedLeft;
	public bool hasSwipedRight;
	public ObjectMenuManager objectMenuManager;
	private bool menuIsSwipable;
	private float menuStickX;

	void Start () {
    //    trackedObj = GetComponent<SteamVR_TrackedObject>();				-> when working with Oculus, this will be off

		if (leftHand)
		{
			thisController = OVRInput.Controller.LTouch;
		} else 
		{
			thisController = OVRInput.Controller.RTouch;
		}
	}
	
	void Update () {
        device = SteamVR_Controller.Input((int)trackedObj.index);

		if (leftHand) 
		{
			menuStickX = OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x;
			if (menuStickX < 0.45f && menuStickX > -0.45f) 
			{
				menuIsSwipable = true;
			}
			if (menuIsSwipable)
			{
				if (menuStickX >= 0.45f)
				{
					//- fire fn that looks at menuList,
					//- disables current item and enables next item

					objectMenuManager.MenuRight ();
					menuIsSwipable = false;
				}
				else if (menuStickX <= -0.45f) {
					objectMenuManager.MenuLeft ();
					menuIsSwipable = false;
				}
			}
		}
		if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, thisController))
		{
			objectMenuManager.SpawnCurrentObject ();
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
		//	if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) 		-> for Vive
			if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, thisController) < 0.1f)		//- checking the grip trigger use the PrimaryIndexTrigger instead
			{
                ThrowObject(other);
            }
			//    else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))	-> For Vive
			else if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, thisController) > 0.1f)
            {
                GrabObject(other);
            }
        }
    }

    void ThrowObject (Collider coli) {

        coli.transform.SetParent(null);
        Rigidbody rigidbody = coli.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
    /*    rigidbody.velocity = device.velocity * throwForce;					-> for Vive
        rigidbody.angularVelocity = device.angularVelocity;		*/
		rigidbody.velocity = OVRInput.GetLocalControllerVelocity (thisController) * throwForce;
		rigidbody.angularVelocity = OVRInput.GetLocalControllerAngularVelocity (thisController).eulerAngles;
        Debug.Log("You have thrown the object");

    }
    void GrabObject (Collider coli) {

        coli.transform.SetParent(gameObject.transform);
        coli.GetComponent<Rigidbody>().isKinematic = true;
        device.TriggerHapticPulse(2000);
        Debug.Log("You are touching down a trigger on an object");

    }
}
