using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	[SerializeField]
	private Transform target;
	private float z_offset;

	// Use this for initialization
	void Start () {
		z_offset = transform.position.z - target.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		var pos = target.position;
		pos.z += z_offset;
		transform.position = pos;
	}
}
