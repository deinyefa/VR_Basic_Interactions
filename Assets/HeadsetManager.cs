using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class HeadsetManager : MonoBehaviour {

	public GameObject viveRig;
	public GameObject oculusRig;
	private bool hmdChosen;

	void Start () {
		if (VRDevice.model == "vive") 
		{
			viveRig.SetActive (true);
			oculusRig.SetActive (false);
			hmdChosen = true;
		}
		else if (VRDevice.model == "oculus")
		{
			oculusRig.SetActive (true);
			viveRig.SetActive (false);
			hmdChosen = true;
		}
	}

	void Update () {
		//- if hmd is chosen after the game has started
		if (!hmdChosen)
		{
			if (VRDevice.model == "vive") 
			{
				viveRig.SetActive (true);
				oculusRig.SetActive (false);
				hmdChosen = true;
			}
			else if (VRDevice.model == "oculus")
			{
				oculusRig.SetActive (true);
				viveRig.SetActive (false);
				hmdChosen = true;
			}
		}
		if (!VRDevice.isPresent) 
		{
			hmdChosen = false;

			//- write code to sync up the CameraRig states
			//- such as position here...
		}
	}
}
