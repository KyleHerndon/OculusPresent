#pragma strict

function Update () {
	transform.rotation.eulerAngles.z = Mathf.Lerp(50, 0, Mathf.Sin(Time.time * Mathf.PI / 1.5));
}