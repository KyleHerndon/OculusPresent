#pragma strict

var slides = new Array();

var CONSTELLATION : float = 45;
var RADIUS : float = 5;
var ROTATION_SPEED :float = 75;
var MAX_SLIDES : int = 4;

var rotateDist : float = -1;
var prefab : GameObject;

function Start () {

}

function Update () {
	if (rotateDist > 0 ) {
		var dA: float = Time.deltaTime * ROTATION_SPEED; 
		for (var s:GameObject in slides) {
			s.transform.RotateAround(Vector3.zero, Vector3.up, dA);
		}
		rotateDist -= dA;
	}
}


function AddToCarousel(slide : Texture) {
	if (rotateDist > 0) {
		Debug.Log("false");
		return false;
	}
	var newSlide : GameObject = Instantiate( prefab );
	newSlide.transform.position = Vector3(-RADIUS,0,0);
	Debug.Log(newSlide.transform.position);
	newSlide.transform.rotation.eulerAngles = Vector3(0, 90, 0);
	newSlide.renderer.materials[0].mainTexture = slide;
	
	slides.Add(newSlide);
	
	if (slides.length > MAX_SLIDES) {
		Debug.Log("remove");
		RemoveFromCarousel();
	}
}

function RemoveFromCarousel() {
	Destroy(slides[0]);
	slides.RemoveAt(0);
}

function RotateCarousel() {
	rotateDist = CONSTELLATION;

}