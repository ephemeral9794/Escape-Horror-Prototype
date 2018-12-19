using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class SelectMenuController : MonoBehaviour {
	[SerializeField]
	Image cursor;

	private int select = 0;

	// Use this for initialization
	void Start () {
		this.OnKeyAsObservable(KeyCode.UpArrow).Subscribe(_ => {
			SelectMenu(1);
		});
		this.OnKeyAsObservable(KeyCode.DownArrow).Subscribe(_ => {
			SelectMenu(0);
		});
		this.OnKeyDownAsObservable(KeyCode.Space).Subscribe(_ => {
			
		});
	}
	
	void SelectMenu(int s)
	{
		select = s;
		switch (s) {
			case 0: { 
					var pos = cursor.GetComponent<RectTransform>().localPosition;
					pos.y = -210.0f;
					cursor.GetComponent<RectTransform>().localPosition = pos; 
				} break;
			case 1: {
					var pos = cursor.GetComponent<RectTransform>().localPosition;
					pos.y = -150.0f;
					cursor.GetComponent<RectTransform>().localPosition = pos;
				} break;
		}
	}
}
