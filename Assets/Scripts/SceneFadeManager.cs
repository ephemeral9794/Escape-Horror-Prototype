using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Image))]
public class SceneFadeManager : MonoBehaviour {
	[SerializeField]
	private float fadeTime = 3.0f;

	private int fadeState;
	private float alpha;
	private Image image;

	private void Awake()
	{
		fadeState = 0;
		alpha = 1;
	}

	void Start () {
		image = GetComponent<Image>();
		image.enabled = true;
		image.color = new Color(0,0,0,1);
		//fadeState = 0;
	}
	
	void Update () {
		switch (fadeState) {
			case 0:
				{ 
					image.color = new Color(0, 0, 0, alpha);
					alpha -= fadeTime * Time.deltaTime;
					if (alpha <= 0.0f) {
						alpha = 0.0f;
						fadeState = 1;
					}
				} break;

			case 1:
				{ 
					image.enabled = false;
				} break;

			case 2:
				{ 
					image.enabled = true;
					image.color = new Color(0, 0, 0, alpha);
					alpha += fadeTime * Time.deltaTime;
					if (alpha >= 1.0f) {
						alpha = 1.0f;
						fadeState = 3;
					}
				} break;

			case 3:
				{
					if (SceneManager.GetActiveScene().name == "Title") {
						SceneManager.LoadScene("Map_Start", LoadSceneMode.Single);
					} else if (SceneManager.GetActiveScene().name == "Map_Start") {
						SceneManager.LoadScene("Map_Entrance", LoadSceneMode.Single);
					}

				} break;
		}
	}

	public void ChangeScene()
	{
		fadeState = 2;
	}
}
