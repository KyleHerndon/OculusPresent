using UnityEngine;
using System.Collections;
using SimpleJSON;

public class GenerateSlide : MonoBehaviour {
	public GameObject prefab;
	public Texture tmpText;
	public TextAsset layout;
	public Texture black;
	public GameObject myoHub;

	private JSONNode slides;
	private int slideMax = 0;
	private int slideIndex = 0;
	private Carousel carousel;
	private bool _locked = false;
	private float _outro = 2f;


	void Awake () {
		carousel = (Carousel) GetComponent<Carousel>();
		slides = JSONNode.Parse(layout.ToString());
		carousel.AddToCarousel(slides["slides"][slideMax]);
		carousel.RotateCarousel();
		slideMax++;
		slideIndex++;
		_locked = true;
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			Advance();
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			Reverse();
		}
		if (_outro < 2f) {
			_outro += Time.deltaTime;
			if (_outro > 1.1f) {
				Destroy(myoHub);
			}
			if (_outro > 1.5f) {
				Application.LoadLevel("Scene2");
			}
		}
	}

	void OnGUI () {
		if (_outro < 1.9f) {
			GUI.color = new Color(1f, 1f, 1f, _outro);
			GUI.DrawTexture(new Rect(-5, -5, Screen.width + 10, Screen.height + 10), black);
		}
	}

	public void Advance () {
		if (!_locked && slideIndex > slides["slides"].Count) {
			_outro = 0;
			print("outch");
			carousel.RotateCarousel();
			_locked = true;
			return;
		}
		if (!_locked && slideIndex <= slides["slides"].Count) {
			if (slideMax == slideIndex) {
				bool added = carousel.AddToCarousel(slides["slides"][slideMax]);
				if (added) {
					slideIndex++;
					slideMax++;
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