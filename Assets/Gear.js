#pragma strict
var scalar : float;
var reversed : boolean;

private var center : Vector3;

function Awake () {
	center = collider.bounds.center;
}

function Update () {
	//transform.rotation.eulerAngles.y += Time.deltaTime * scalar;
	if (reversed)
		transform.RotateAround(center, Vector3.up, Time.deltaTime * scalar);
	else
		transform.RotateAround(center,Vector3.down, Time.deltaTime * scalar);
}