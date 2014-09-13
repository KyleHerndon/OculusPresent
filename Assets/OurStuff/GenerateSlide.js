#pragma strict

import SimpleJSON;

var prefab   : GameObject;
var tmpText  : Texture;
var layout   : TextAsset;

private var slides:JSONNode;

private var carousel : Carousel;


function Start () {
	carousel = GetComponent(Carousel) as Carousel;
	slides = JSONNode.Parse(layout.ToString());
	Debug.Log(slides);
	var url:String = "https://www.google.com/images/srpr/logo11w.png";
	var www:WWW = new WWW(url);
	yield www;
	tmpText = www.texture;
}

function Update () {
	if (Input.GetKeyDown("right")) {
		Debug.Log("right");
		var added : boolean = carousel.AddToCarousel(tmpText, slides["slides"][0]);
		if (added) {
			carousel.RotateCarousel();
		}
		
	}

}
