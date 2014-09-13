#pragma strict

import SimpleJSON;

var prefab   : GameObject;
var tmpText  : Texture;
var layout   : TextAsset;

private var slides:JSONNode;

private var slideN:int = 0;

private var carousel : Carousel;


function Start () {
	carousel = GetComponent(Carousel) as Carousel;
	slides = JSONNode.Parse(layout.ToString());
	Debug.Log(slides);
	carousel.AddToCarousel(slides["slides"][slideN]);
	carousel.RotateCarousel();
	slideN++;
}

function Update () {
	if (Input.GetKeyDown("right") && slideN < slides["slides"].Count) {
		var added : boolean = carousel.AddToCarousel(slides["slides"][slideN]);
		Debug.Log(added);
		carousel.RotateCarousel();
		if (added) {
			slideN++; 
		}
		
	}

}
