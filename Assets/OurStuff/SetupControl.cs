using UnityEngine;
using System.Collections;

public class SetupControl : MonoBehaviour {

	private bool showGui = true;
	private string text = "";
	private bool click = false;

	public GameObject slideshow;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
			
	}

	void OnGUI() {
		GUI.Box (new Rect(Screen.width/2 - 250, Screen.height/2+200, 500, 100), GUIContent.none);
		text = GUI.TextArea(new Rect(Screen.width/2 - 225, Screen.height/2+225, 300, 50), text);
		click = GUI.Button (new Rect(Screen.width/2 + 100, Screen.height/2+225, 125, 50), "Load App");
		if (click) {
			slideshow.SendMessage("LoadFile",text);
			this.transform.gameObject.SetActive(false);
		}
	}
}
