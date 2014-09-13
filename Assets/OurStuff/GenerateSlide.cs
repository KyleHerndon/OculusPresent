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


	void Awake () {
		carousel = (Carousel) GetComponent<Carousel>();
		slides = JSONNode.Parse(layout.ToString());
		carousel.AddToCarousel(slides["slides"][slideN]);
		carousel.RotateCarousel();
		slideN++;
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