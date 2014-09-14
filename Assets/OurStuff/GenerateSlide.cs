using UnityEngine;
using System.Collections;
using SimpleJSON;

public class GenerateSlide : MonoBehaviour {
	public GameObject prefab;
	public Texture tmpText;
	public TextAsset layout;

	private JSONNode slides;
	private int slideN = 0;
	private Carousel carousel;

	private GameObject model;


	void Awake () {
		carousel = (Carousel) GetComponent<Carousel>();
		slides = JSONNode.Parse(layout.ToString());
		carousel.AddToCarousel(slides["slides"][slideN]);
		carousel.RotateCarousel();
		slideN++;

		model = RenderModel("http://www.everyday3d.com/unity3d/obj/monkey.obj");
	}

	GameObject RenderModel(string url) {
		GameObject newObj = new GameObject();
		newObj.AddComponent<OBJ>();	
		OBJ obj = newObj.GetComponent<OBJ>();
		obj.objPath = url;

		return newObj;
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			Advance();
		}
	}

	public void Advance () {
		if (slideN < slides["slides"].Count) {
			bool added = carousel.AddToCarousel(slides["slides"][slideN]);
			Debug.Log(added);
			carousel.RotateCarousel();
			if (added) {
				slideN++; 
			}
		}
	}
}