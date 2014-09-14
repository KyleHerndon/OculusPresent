using UnityEngine;
using System.Collections;

public class FreeformMotion : MonoBehaviour {
	public Transform[] obj;
	public Vector3[] cameraPos;

	public int animationTime;
	public GameObject myoHub;
	public Texture blackout;
	
	private int _index = 0;
	private int _animation = 0;
	private float _time = 0f;
	private Quaternion _rotationStart;
	private Quaternion _rotationEnd;
	private Vector3 _camStart;
	private Vector3 _camEnd;
	private OVRCardboard cardboard;
	private int _direction;
	
	private const int FORWARD = -1;
	private const int BACKWARD = 1;
	
	void Awake () {
		// Find the cardboard controller
		cardboard = transform.FindChild ("CameraRight").GetComponent<OVRCardboard> ();
	}
	
	void Update () {
		switch (_animation) {
		case 1:
			if (_time > animationTime / 2) {
				_animation = 2;
				cardboard.model = obj[_index].gameObject;
				obj[_index].rotation = obj[_index + _direction].rotation;
			} else {
			}
			goto case 2;
		case 2:
			if (_time > animationTime / 2) {
			}
			_time += Time.deltaTime;
			obj[_index + _direction].position = Vector3.Lerp (_stage, _end, timeFunc(_time / animationTime));
			obj[_index].position = Vector3.Lerp (_start, _stage, timeFunc(_time / animationTime));
			transform.position = Vector3.Lerp (_camStart, _camEnd, timeFunc(_time / animationTime));
			break;
			// Outro
		case -1:
			_time += Time.deltaTime;
			obj[_index].position = Vector3.Lerp (_start, _end, timeFunc(_time / animationTime));
			transform.position = Vector3.Lerp (_camStart, _camEnd, timeFunc(_time / animationTime));
			break;
		}
		if (_time > animationTime) {
			if (_animation == -1) {
				Destroy(myoHub);
				Application.LoadLevel ("Scene4");
			}
			_animation = 0;
			_time = 0f;
		}
	}
	
	void OnGUI () {
		if (_animation == -1) {
			GUI.color = new Color(1f, 1f, 1f, _time / animationTime);
			GUI.DrawTexture(new Rect(-5, -5, Screen.width + 10, Screen.height + 10), blackout);
		}
	}
	
	public void Advance () {
		if (_animation == 0) {
			if (++_index < obj.Length) {
				_animation = 1;
				_time = 0f;
				setUpAnimation(FORWARD);
			} else {
				_index--;
				_animation = -1;
				_start = obj[_index].position;
				_end = _start + Vector3.left * (gap / 2);
				animationTime /= 2;
				_time = 0f;
				_camStart = transform.position;
				_camEnd = Vector3.back * gap;
			}
		}
	}
	
	public void Reverse () {
		if (_animation == 0) {
			if (--_index >= 0) {
				_animation = 1;
				_time = 0f;
				setUpAnimation(BACKWARD);
			} else {
				_index = 0;
			}
		}
	}
	
	private void setUpAnimation(int direction) {
		_direction = direction;
		_camStart = transform.position;
		_camEnd = cameraPos[_index];
		_rotationStart = transform.rotation;
		_rotationEnd = Quaternion
	}
	
	private float timeFunc (float completion) {
		completion = Mathf.Clamp01 (completion);
		return Mathf.Sin (completion * Mathf.PI / 2);
	}
}
