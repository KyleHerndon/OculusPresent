using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public Transform[] obj;
	public Vector3[] cameraPos;

	public int animationTime;
	public GameObject myoHub;
	public Texture blackout;
	
	private int _index = -1;
	private int _animation = 0;
	private float _time = 0f;
	private Quaternion _rotationStart;
	private Quaternion _rotationEnd;
	private Vector3 _camStart;
	private Vector3 _camEnd;
	private int _direction;
	
	private const int FORWARD = -1;
	private const int BACKWARD = 1;
	
	void Awake () {
		foreach(Transform t in obj) {
			t.gameObject.SetActive(false);
		}
	}
	
	void Update () {
		switch (_animation) {
		case 1:
			if (_time > animationTime / 2) {
				_animation = 2;
			} else {
			}
			goto case 2;
		case 2:
			if (_time > animationTime / 2) {
			}
			_time += Time.deltaTime;
			transform.position = Vector3.Lerp (_camStart, _camEnd, timeFunc(_time / animationTime));
			transform.rotation = Quaternion.Lerp (_rotationStart, _rotationEnd, timeFunc(_time / animationTime));
			break;
			// Outro
		case -1:
			_time += Time.deltaTime;
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
				obj[_index].gameObject.SetActive(true);
//				if (_index == 3) {
//					obj[_index + 1].gameObject.SetActive(true);
//				} else if (_index == 4) {
//					obj[++_index].gameObject.SetActive(true);
//				}
				setUpAnimation(FORWARD);
			} else {
				_index--;
				_animation = -1;
				animationTime /= 2;
				_time = 0f;
				_camStart = transform.position;
				_camEnd = Vector3.back;
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
		_rotationEnd = Quaternion.LookRotation(obj[_index].position - _camEnd);
	}

	private float timeFunc (float completion) {
		completion = Mathf.Clamp01 (completion);
		return Mathf.Sin (completion * Mathf.PI / 2);
	}
}
