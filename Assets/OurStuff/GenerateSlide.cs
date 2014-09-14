using UnityEngine;
using System.Collections;
using SimpleJSON;

public class GenerateSlide : MonoBehaviour {
	public GameObject prefab;
	public Texture tmpText;
	public TextAsset layout;

	private JSONNode slides;
	private int slideMax = 0;
	private int slideIndex = 0;
	private Carousel carousel;
	private bool _locked = false;
	private float _outro = -1f;


	void Awake () {
		carousel = (Carousel) GetComponent<Carousel>();
		slides = JSONNode.Parse(layout.ToString());
		carousel.AddToCarousel(slides["slides"][slideMax]);
		carousel.RotateCarousel();
		slideMax++;
		slideIndex++;
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			Advance();
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			Reverse();
		}
		if (_outro > 0) {
			_outro -= Time.deltaTime / 1.5f;
			if (_outro < -.5f) {
				Application.LoadLevel("Scene1");
			}
		}
	}

	void OnGUI () {
		if (_outro > 0) {
			GUI.color = new Color(1f, 1f, 1f, _outro);
		}
	}

	public void Advance () {
		if (!_locked && slideIndex <= slides["slides"].Count) {
			if (slideMax == slideIndex) {
				bool added = carousel.AddToCarousel(slides["slides"][slideMax]);
				if (added) {
					slideIndex++;
					slideMax++;
				} else {
					_outro = 1;
				}
			} else { slideIndex++; }
			carousel.RotateCarousel();
			_locked = true;
			print(slideIndex);
		}
	}

	public void Reverse () {
		if (!_locked && slideIndex > 1) {
			carousel.RotateCarousel(true);
			slideIndex--;
			_locked = true;
		}
	}

	public void Refresh () {
		_locked = false;
	}
}