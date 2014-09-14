#pragma strict
var angle : float;

function FixedUpdate () {
	transform.RotateAround(transform.position, Vector3.up, angle);
}