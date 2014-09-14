using UnityEngine;
using System;
using System.Collections;
using Pose = Thalmic.Myo.Pose;
using VibrationType = Thalmic.Myo.VibrationType;

public class MyoCardboardController : MonoBehaviour
{
	// Myo game object to connect with.
	// This object must have a ThalmicMyo script attached.
	public GameObject myo = null;
	public CardboardMultiplex CM;
	
	// Pose variables ---------------
	private Pose _lastPose = Pose.Unknown;
	
	void Update () {
		// Access the ThalmicMyo component attached to the Myo game object.
		ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();
		
		UpdateKeyPresses (thalmicMyo);
		UpdatePoses (thalmicMyo);
	}
	
	// Draw some basic instructions.
	void OnGUI ()
	{
		GUI.skin.box.fontSize = 20;
		GUI.backgroundColor = new Color(1f, 1f, 1f, .7f);
		
		ThalmicHub hub = ThalmicHub.instance;
		ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();
		
		String text = "";
		if (!hub.hubInitialized) {
			text = "Cannot contact Myo Connect.\n" +
				"Press Q to try again.";
		} else if (!thalmicMyo.isPaired) {
			text = "No Myo currently paired.";
		} else if (!thalmicMyo.armRecognized) {
			text = "Please perform the Setup Gesture.";
		} else {
			return;
		}
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		GUI.Box(new Rect (0, 0, Screen.width/2, Screen.height), text);
		GUI.Box(new Rect (Screen.width/2, 0, Screen.width/2, Screen.height), text);
	}
	
	void UpdateKeyPresses (ThalmicMyo thalmicMyo) {
		ThalmicHub hub = ThalmicHub.instance;
		
		if (Input.GetKeyDown (KeyCode.Q)) {
			hub.ResetHub();
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			CM.Advance();
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			CM.Reverse();
		}
	}
	
	void UpdatePoses (ThalmicMyo thalmicMyo) {
		if (thalmicMyo.pose != _lastPose && thalmicMyo.pose != Pose.Unknown) {
			
			_lastPose = thalmicMyo.pose;
			
			if (thalmicMyo.pose == Pose.Rest) {
				return;
			}
			
			// Give feedback
			thalmicMyo.Vibrate (VibrationType.Short);
			
			if (thalmicMyo.pose == Pose.Fist) {
				print("Fist detected");
				
			} else if (thalmicMyo.pose == Pose.WaveIn) {
				if (thalmicMyo.arm == Thalmic.Myo.Arm.Left) {
					print("Wave RIGHT detected (left arm)");
					CM.Reverse ();
					
				} else if (thalmicMyo.arm == Thalmic.Myo.Arm.Right) {
					print("Wave LEFT detected (right arm)");
					CM.Advance ();
				}
				
			} else if (thalmicMyo.pose == Pose.WaveOut) {
				if (thalmicMyo.arm == Thalmic.Myo.Arm.Left) {
					print("Wave LEFT detected (left arm)");
					CM.Advance ();
					
				} else if (thalmicMyo.arm == Thalmic.Myo.Arm.Right) {
					print("Wave RIGHT detected (right arm)");
					CM.Reverse ();
				}
				
			} else if (thalmicMyo.pose == Pose.ThumbToPinky) {
				print("Thumb to Pinky detected");
				
			} else if (thalmicMyo.pose == Pose.FingersSpread) {
				print("Fingers Spread detected");

			}
		}
	}
}