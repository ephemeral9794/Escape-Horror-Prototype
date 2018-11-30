using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private float timeElasped;

	// Use this for initialization
	void Start () {
		timeElasped = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
		timeElasped += Time.deltaTime;
	}
}
