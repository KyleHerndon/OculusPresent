using UnityEngine;
using System.Collections;
using Pose = Thalmic.Myo.Pose;
using VibrationType = Thalmic.Myo.VibrationType;

public class MyoController : MonoBehaviour
{
	// Myo game object to connect with.
	// This object must have a ThalmicMyo script attached.
	public GameObject myo = null;

	// Pose variables ---------------
	private Pose _lastPose = Pose.Unknown;
	public bool showPointer = false;
	public bool grabEnabled = true;
	private Transform _grab = null;

	// Orientation variables ---------
	// Update references when the pose becomes fingers spread or the q key is pressed.
	private bool _updateReference = false;
	// Calibration
	private Quaternion _antiYaw = Quaternion.identity;
	private float _referenceRoll = 0.0f;

	private bool _updateGrabReference = false;
	private Quaternion _grabInitialObjectRotation;
	private Quaternion _grabInitialPointerRotation;
	private GameObject _laser;

	void Awake () {
		// Set up light
		_laser = new GameObject("Laser Pointer");
		_laser.transform.parent = this.transform;
		_laser.AddComponent<Light>();
		_laser.light.color = Color.red;
		_laser.light.intensity = 0f;
		_laser.light.range = 100f;
		_laser.light.type = LightType.Spot;
		_laser.transform.position = this.transform.position;
	}

	void Update () {
		// Access the ThalmicMyo component attached to the Myo game object.
		ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();

		UpdateKeyPresses (thalmicMyo);
		UpdatePoses (thalmicMyo);
		UpdateOrientation (thalmicMyo);
		Render();
	}

	// Draw some basic instructions.
	void OnGUI ()
	{
		GUI.skin.label.fontSize = 20;
		
		ThalmicHub hub = ThalmicHub.instance;
		ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();
		
		if (!hub.hubInitialized) {
			GUI.Label(new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2),
			          "Cannot contact Myo Connect. Is Myo Connect running?\n" +
			          "Press Q to try again."
			          );
		} else if (!thalmicMyo.isPaired) {
			GUI.Label(new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2),
			          "No Myo currently paired."
			          );
		} else if (!thalmicMyo.armRecognized) {
			GUI.Label(new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2),
			          "Please perform the Setup Gesture."
			          );
		} else {
			/*GUI.Label (new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2),
			           "Fist: Vibrate Myo armband\n" +
			           "Wave in: Set box material to blue\n" +
			           "Wave out: Set box material to green\n" +
			           "Thumb to pinky: Reset box material\n" +
			           "Fingers spread: Set forward direction"
			           );*/
		}
	}

	void Render() {
		if(showPointer && _grab == null) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.rotation * Vector3.forward, out hit, 100f)) {
				_laser.transform.position = Vector3.MoveTowards(hit.point, this.transform.parent.position, .5f);
				_laser.transform.rotation = Quaternion.LookRotation((hit.point - _laser.transform.position).normalized);
				_laser.light.intensity = 1f;
			}
		} else {
			_laser.light.intensity = 0f;
		}
	}

	void UpdateKeyPresses (ThalmicMyo thalmicMyo) {
		ThalmicHub hub = ThalmicHub.instance;
		
		if (Input.GetKeyDown (KeyCode.Q)) {
			hub.ResetHub();
		} else if (Input.GetKeyDown (KeyCode.R)) {
			_updateReference = true;
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
				if (grabEnabled) {
					if (_grab == null) {
						RaycastHit hit;
						if (Physics.Raycast (transform.position, transform.rotation * Vector3.forward, out hit, 100f)) {
							_grab = hit.collider.transform;
						}
						_updateGrabReference = true;
					} else {
						_grab = null;
					}
				}

			} else if (thalmicMyo.pose == Pose.WaveIn) {
				if (thalmicMyo.arm == Thalmic.Myo.Arm.Left) {
					print("Wave RIGHT detected (left arm)");
				} else if (thalmicMyo.arm == Thalmic.Myo.Arm.Right) {
					print("Wave LEFT detected (right arm)");
				}

			} else if (thalmicMyo.pose == Pose.WaveOut) {
				if (thalmicMyo.arm == Thalmic.Myo.Arm.Left) {
					print("Wave LEFT detected (left arm)");
				} else if (thalmicMyo.arm == Thalmic.Myo.Arm.Right) {
					print("Wave RIGHT detected (right arm)");
				}

			} else if (thalmicMyo.pose == Pose.ThumbToPinky) {
				print("Thumb to Pinky detected");
				if (Mathf.Approximately (_referenceRoll, 0f)) {
					_updateReference = true;
					print("Calibrated");
				}

			} else if (thalmicMyo.pose == Pose.FingersSpread) {
				print("Fingers Spread detected");
				showPointer = !showPointer;
			}
		}
	}

	void UpdateOrientation (ThalmicMyo thalmicMyo) {
		// Update references. This anchors the joint on-screen such that it faces forward away
		// from the viewer when the Myo armband is oriented the way it is when these references are taken.
		if (_updateReference) {
			// _antiYaw represents a rotation of the Myo armband about the Y axis (up) which aligns the forward
			// vector of the rotation with Z = 1 when the wearer's arm is pointing in the reference direction.
			_antiYaw = Quaternion.FromToRotation (
				new Vector3 (myo.transform.forward.x, 0, myo.transform.forward.z),
				new Vector3 (0, 0, 1)
				);
			
			// _referenceRoll represents how many degrees the Myo armband is rotated clockwise
			// about its forward axis (when looking down the wearer's arm towards their hand) from the reference zero
			// roll direction. This direction is calculated and explained below. When this reference is
			// taken, the joint will be rotated about its forward axis such that it faces upwards when
			// the roll value matches the reference.
			Vector3 referenceZeroRoll = computeZeroRollVector (myo.transform.forward);
			_referenceRoll = rollFromZero (referenceZeroRoll, myo.transform.forward, myo.transform.up);
			_updateReference = false;
		} else if (_updateGrabReference) {
			_grabInitialObjectRotation = _grab.transform.rotation;
			_grabInitialPointerRotation = transform.rotation;
			_updateGrabReference = false;
		}

		// Current zero roll vector and roll value.
		Vector3 zeroRoll = computeZeroRollVector (myo.transform.forward);
		float roll = rollFromZero (zeroRoll, myo.transform.forward, myo.transform.up);
		
		// The relative roll is simply how much the current roll has changed relative to the reference roll.
		// adjustAngle simply keeps the resultant value within -180 to 180 degrees.
		float relativeRoll = normalizeAngle (roll - _referenceRoll);
		
		// antiRoll represents a rotation about the myo Armband's forward axis adjusting for reference roll.
		Quaternion antiRoll = Quaternion.AngleAxis (relativeRoll, myo.transform.forward);
		
		// Here the anti-roll and yaw rotations are applied to the myo Armband's forward direction to yield
		// the orientation of the joint.
		transform.rotation = _antiYaw * antiRoll * Quaternion.LookRotation (myo.transform.forward);
		
		// The above calculations were done assuming the Myo armbands's +x direction, in its own coordinate system,
		// was facing toward the wearer's elbow. If the Myo armband is worn with its +x direction facing the other way,
		// the rotation needs to be updated to compensate.
		if (thalmicMyo.xDirection == Thalmic.Myo.XDirection.TowardWrist) {
			// Mirror the rotation around the XZ plane in Unity's coordinate system (XY plane in Myo's coordinate
			// system). This makes the rotation reflect the arm's orientation, rather than that of the Myo armband.
			transform.rotation = new Quaternion(transform.localRotation.x,
			                                    -transform.localRotation.y,
			                                    transform.localRotation.z,
			                                    -transform.localRotation.w);
		}

		if (_grab != null) {
			_grab.rotation = Quaternion.Inverse (transform.rotation) * _grabInitialPointerRotation * _grabInitialObjectRotation;
		}
	}

	// Compute the angle of rotation clockwise about the forward axis relative to the provided zero roll direction.
	// As the armband is rotated about the forward axis this value will change, regardless of which way the
	// forward vector of the Myo is pointing. The returned value will be between -180 and 180 degrees.
	float rollFromZero (Vector3 zeroRoll, Vector3 forward, Vector3 up) {
		// The cosine of the angle between the up vector and the zero roll vector. Since both are
		// orthogonal to the forward vector, this tells us how far the Myo has been turned around the
		// forward axis relative to the zero roll vector, but we need to determine separately whether the
		// Myo has been rolled clockwise or counterclockwise.
		float cosine = Vector3.Dot (up, zeroRoll);

		// To determine the sign of the roll, we take the cross product of the up vector and the zero
		// roll vector. This cross product will either be the same or opposite direction as the forward
		// vector depending on whether up is clockwise or counter-clockwise from zero roll.
		// Thus the sign of the dot product of forward and it yields the sign of our roll value.
		Vector3 cp = Vector3.Cross (up, zeroRoll);
		float directionCosine = Vector3.Dot (forward, cp);
		float sign = directionCosine < 0.0f ? 1.0f : -1.0f;

		// Return the angle of roll (in degrees) from the cosine and the sign.
		return sign * Mathf.Rad2Deg * Mathf.Acos (cosine);
	}

	// Compute a vector that points perpendicular to the forward direction,
	// minimizing angular distance from world up (positive Y axis).
	// This represents the direction of no rotation about its forward axis.
	Vector3 computeZeroRollVector (Vector3 forward) {
		Vector3 antigravity = Vector3.up;
		Vector3 m = Vector3.Cross (myo.transform.forward, antigravity);
		Vector3 roll = Vector3.Cross (m, myo.transform.forward);

		return roll.normalized;
	}

	// Adjust the provided angle to be within a -180 to 180.
	float normalizeAngle (float angle) {
		if (angle > 180.0f) {
			return angle - 360.0f;
		}
		if (angle < -180.0f) {
			return angle + 360.0f;
		}
		return angle;
	}
}