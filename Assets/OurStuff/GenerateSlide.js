#pragma strict

var prefab   : GameObject;
var tmpText  : Texture;

private var carousel : Carousel;

function Start () {
	carousel = GetComponent(Carousel) as Carousel;
}

function Update () {
	if (Input.GetKeyDown("right")) {
		Debug.Log("right");
		carousel.AddToCarousel(tmpText);
		carousel.RotateCarousel();
		
	}

}
