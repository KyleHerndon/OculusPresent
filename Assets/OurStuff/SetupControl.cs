using UnityEngine;
using System.Collections;

public class SetupControl : MonoBehaviour {
	private bool showGui = true;
	private string text = "";
	private bool click = false;

	public GameObject slideshow;
	public GameObject plane;
	public GUISkin skin;

	void Update () {
		if(Input.GetKeyDown(KeyCode.Return)) {
			slideshow.SendMessage("LoadFile",text);
			plane.SetActive(true);
			gameObject.SetActive(false);
		}
	}

	void OnGUI() {
		GUI.skin = skin;
		GUI.Box (new Rect(Screen.width/2 - 250, Screen.height/2+200, 500, 100), GUIContent.none);
		text = GUI.TextField(new Rect(Screen.width/2 - 225, Screen.height/2+225, 300, 50), text);
		click = GUI.Button (new Rect(Screen.width/2 + 100, Screen.height/2+225, 125, 50), "Load App");
		if (click) {
			slideshow.SendMessage("LoadFile",text);
			plane.SetActive(true);
			gameObject.SetActive(false);
		}
	}
}
