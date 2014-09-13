#pragma strict

import TextSize;

var slides = new Array();

var CONSTELLATION : float = 70;
var RADIUS : float = 5;
var ROTATION_SPEED :float = 75;
var MAX_SLIDES : int = 4;

var rotateDist : float = -1;
var prefab : GameObject;

var titleFab : GameObject;
var bodyFab  : GameObject;

var defaultT : Texture;

private var ts:TextSize;

function Start () {
}

function Update () {
	Debug.Log(rotateDist);
	if (rotateDist > 0 ) {
		var dA: float = Time.deltaTime * ROTATION_SPEED; 
		for (var s:GameObject in slides) {
			s.transform.RotateAround(Vector3.zero, Vector3.up, dA);
		}
		rotateDist -= dA;
	}
}


function AddToCarousel( params:JSONNode) {
	if (params["type"] && params["type"].ToString() == "\"slide\"") {
		var slide : Texture = defaultT;
		
		if (rotateDist > 0) {
			return false;
		}
		var newSlide : GameObject = Instantiate( prefab );
		newSlide.transform.position = Vector3(-RADIUS,0,0);
		newSlide.transform.rotation.eulerAngles = Vector3(0, 90, 0);
		newSlide.renderer.materials[0].mainTexture = slide;
		
		if (params["background-color"]) {
			Debug.Log(params["background-color"].ToString().Split("\""[0])[1]);
			var cols : String[] = params["background-color"].ToString().Split("\""[0])[1].Split(","[0]); 
			var r : float = parseFloat(cols[0]);
			var g : float = parseFloat(cols[1]);
			var b : float = parseFloat(cols[2]);
			var a : float = 1;
			if (cols.Length == 4) {
				a = parseFloat(cols[3]);
			} 
			newSlide.renderer.material.color = new Color(r,g,b,a);
		}
		
		var tm:TextMesh;
		
		if (params["title"]) {
			var newTitle : GameObject = Instantiate( titleFab );
			tm = (newTitle.GetComponent(TextMesh) as TextMesh);
			tm.text = params["title"];
			var bounds:Bounds = tm.renderer.bounds;
			newTitle.transform.localScale.x = 1.0/bounds.size.z * newSlide.transform.localScale.x - 0.1;
			newTitle.transform.parent = newSlide.transform;
			newTitle.transform.localPosition = Vector3( 0 , 0.4, 0.5);
		}
		
		if (params["text"]) {
			
			var newBody : GameObject = Instantiate( bodyFab );
			tm = (newBody.GetComponent(TextMesh) as TextMesh);
			newBody.transform.localScale.x = newSlide.transform.localScale.x*.1;
			newBody.transform.localScale.y = newSlide.transform.localScale.y*.1;
			newBody.transform.parent = newSlide.transform;
			newBody.transform.localPosition = Vector3( 0.46, 0.25, 0.5);
			ts = new TextSize(tm);
			var words:String[] = params["text"].ToString().Split(" "[0]);
			var fin  : String = "";
			var oldS : String = "";
			var newS : String = "";
			for (var s : String in words) {
				newS += s+" ";
				if (ts.GetTextWidth(newS) > newSlide.transform.localScale.x*0.92) {
					fin+=oldS+"\n";
					newS = s+" ";
					oldS = "";
				} else if (s.IndexOf("\\n") > 0) {
					oldS+=s.Substring(0, s.IndexOf("\\n"))+"\n\n"; 
					fin+=oldS;
					newS = "";
					oldS = "";
				} else {
					oldS = newS;
				}
			}
			fin += newS;
			tm.text = fin;//.Replace("\\n", "\n");
			
			newBody.transform.rotation.eulerAngles = Vector3(0 , -90, 0);
			newBody.transform.localScale.x = .1;
				
		}
		
		slides.Add(newSlide);
		
		if (slides.length > MAX_SLIDES) {
			RemoveFromCarousel();
		}
		
		return true;
	} else if (params["type"] && params["type"].ToString() == "\"model\"") {
		var newObj = new GameObject();
		newObj.AddComponent(OBJ);	
		
		var obj : OBJ = newObj.GetComponent(OBJ) as OBJ;
		
		obj.objPath = params["url"];
		return true;
		
	}
}

function RemoveFromCarousel() {
	Destroy(slides[0]);
	slides.RemoveAt(0);
}

function RotateCarousel() {
	if (rotateDist < 0) {
		rotateDist = CONSTELLATION;
	}

}