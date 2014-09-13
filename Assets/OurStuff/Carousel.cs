using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using TextSize;

public class Carousel : MonoBehaviour
{
	public List<GameObject> slides = new List<GameObject>();
	public float CONSTELLATION = 90f;
	public float RADIUS = 5.5f;
	public float ROTATION_SPEED = 75f;
	public int MAX_SLIDES = 4;

	public float rotateDist = -1f;
	public GameObject prefab;

	public GameObject titleFab;
	public GameObject bodyFab;

	public Texture defaultT;

	TextSize.TextSize ts;

	void Update () {
		// Debug.Log(rotateDist);
		if (rotateDist > 0 ) {
			float dA = Time.deltaTime * ROTATION_SPEED; 
			foreach (GameObject s in slides) {
				s.transform.RotateAround(Vector3.zero, Vector3.up, dA);
			}
			rotateDist -= dA;
		}
	}

	public bool AddToCarousel(JSONNode args) {
		if (args["type"] != null && args["type"].ToString() == "\"slide\"") {
			Texture slide = defaultT;

			if (rotateDist > 0) {
				return false;
			}
			GameObject newSlide = (GameObject) Instantiate( prefab );
			newSlide.transform.position = new Vector3(-RADIUS,0,0);
			newSlide.transform.rotation.eulerAngles.Set(0, 90, 0);
			newSlide.renderer.materials[0].mainTexture = slide;
			
			if (args["background-color"] != null) {
				Debug.Log(args["background-color"].ToString().Split("\""[0])[1]);
				String[] cols = args["background-color"].ToString().Split("\""[0])[1].Split(","[0]); 
				float r = float.Parse(cols[0]);
				float g = float.Parse(cols[1]);
				float b = float.Parse(cols[2]);
				float a = 1;
				if (cols.Length == 4) {
					a = float.Parse(cols[3]);
				} 
				newSlide.renderer.material.color = new Color(r,g,b,a);
			}

			TextMesh tm;

			if (args["title"] != null) {
				GameObject newTitle = (GameObject) Instantiate( titleFab );
				tm = newTitle.GetComponent<TextMesh>();
				tm.text = args["title"];
				Bounds bounds = tm.renderer.bounds;
				newTitle.transform.localScale = new Vector3(1f/bounds.size.z * newSlide.transform.localScale.x - 0.1f, newTitle.transform.localScale.y, newTitle.transform.localScale.z);
				newTitle.transform.parent = newSlide.transform;
				newTitle.transform.localPosition = new Vector3( 0f , 0.4f, 0.5f);
			}

			if (args["text"] != null) {

				GameObject newBody = (GameObject) Instantiate( bodyFab );
				tm = newBody.GetComponent<TextMesh>();
				newBody.transform.localScale = new Vector3(newSlide.transform.localScale.x*.1f, newSlide.transform.localScale.y*.1f, newBody.transform.localScale.z);
				newBody.transform.parent = newSlide.transform;
				newBody.transform.localPosition = new Vector3( 0.46f, 0.25f, 0.5f);
				ts = new TextSize.TextSize(tm);
				String[] words	= args["text"].ToString().Split(' ');
				String fin	= "";
				String oldS	= "";
				String newS	= "";
				foreach (String s in words) {
					newS += s+" ";
					if (ts.GetTextWidth(newS) > newSlide.transform.localScale.x*0.92f) {
						fin +=oldS+"\n";
						newS = s+" ";
						oldS = "";
					} else if (s.IndexOf("\\n") > 0) {
						oldS	+= s.Substring(0, s.IndexOf("\\n"))+"\n\n";
						fin		+= oldS;
						newS	=  "";
						oldS	=  "";
					} else {
						oldS = newS;
					}
				}
				fin += newS;
				tm.text = fin;//.Replace("\\n", "\n");

				newBody.transform.eulerAngles = new Vector3(0, -90, 0);
				newBody.transform.localScale = new Vector3(.1f, newBody.transform.localScale.y, newBody.transform.localScale.z);

			}

			slides.Add(newSlide);

			if (slides.Count > MAX_SLIDES) {
				RemoveFromCarousel();
			}
			
			return true;
		} else if (args["type"] != null && args["type"].ToString() == "\"model\"") {
			GameObject newObj = new GameObject();
			newObj.AddComponent<OBJ>();	
			
			OBJ obj = newObj.GetComponent<OBJ>();
			
			obj.objPath = args["url"];
			return true;
		}
		return false;
	}

	public void RemoveFromCarousel() {
		Destroy(slides[0]);
		slides.RemoveAt(0);
	}

	public void RotateCarousel() {
		if (rotateDist < 0) {
			rotateDist = CONSTELLATION;
		}
	}
}