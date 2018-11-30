using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SceneFadeManager : MonoBehaviour {
	public static int FadeState { get; set; }

	[SerializeField]
	private float fadeTime = 3.0f;

	private Image image;

	// Use this for initialization
	void Start () {
		image = GetComponent<Image>();
		image.enabled = true;
		image.color = new Color(0,0,0,1);
	}
	
	// Update is called once per frame
	void Update () {
		switch (FadeState) {
			case 0: {
					image.CrossFadeAlpha(0.0f, fadeTime, false);
					FadeState = 1;
				} break;
		}
	}
}
