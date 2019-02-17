using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace EscapeHorror.Prototype { 
	public class SelectMenuController : MonoBehaviour {
		enum Menu
		{
			Start, Desc, Exit
		}

		[SerializeField]
		private GameObject descBoard;
		[SerializeField]
		private int nextScene;
	
		private Image cursor;
		private GameManager manager;
		private Menu select = 0;
		private GameObject instance;
		private AudioSource se;
		private bool flag;

		// Use this for initialization
		void Start () {
			flag = true;
			cursor = GetComponentInChildren<Image>();
			manager = FindObjectOfType<GameManager>();
			se = GetComponent<AudioSource>();
			var fadeManager = FindObjectOfType<SceneFadeManager>();
			fadeManager.ObserveEveryValueChanged((m) => m.fadeState).Where((s) => s == 1).Subscribe((_) => {
				this.OnKeyDownAsObservable(KeyCode.UpArrow).Subscribe(__ => {
					if (flag) { 
						se.Play();
						UpMenu();
					}
				});
				this.OnKeyDownAsObservable(KeyCode.DownArrow).Subscribe(__ => {
					if (flag) {
						se.Play();
						DownMenu();
					}
				});
				Action<Unit> action = (__) => {
					se.Play();
					switch (select)
					{
						case Menu.Start:
							manager.ChangeScene(nextScene);
							break;
						case Menu.Desc:
							if (flag) {
								instance = Instantiate(descBoard, transform.parent);
								instance.OnDestroyAsObservable().Subscribe(___ => flag = true);
								flag = false;
							}
							break;
						case Menu.Exit:
							Application.Quit();
							break;
					}
				};
				this.OnKeyDownAsObservable(KeyCode.Space).Subscribe(action);
				this.OnKeyDownAsObservable(KeyCode.Return).Subscribe(action);
			}).AddTo(this);
		}

		void DownMenu()
		{
			switch(select)
			{
				case Menu.Start:
					select = Menu.Desc;
					break;
				case Menu.Desc:
					select = Menu.Exit;
					break;
				case Menu.Exit:
					select = Menu.Start;
					break;
			}
			ChangePosition();
		}
		void UpMenu()
		{
			switch (select)
			{
				case Menu.Start:
					select = Menu.Exit;
					break;
				case Menu.Desc:
					select = Menu.Start;
					break;
				case Menu.Exit:
					select = Menu.Desc;
					break;
			}
			ChangePosition();
		}

		void ChangePosition()
		{
			switch (select) {
				case Menu.Start: { 
						var pos = cursor.GetComponent<RectTransform>().localPosition;
						pos.y = 0;
						cursor.GetComponent<RectTransform>().localPosition = pos; 
					} break;
				case Menu.Desc: {
						var pos = cursor.GetComponent<RectTransform>().localPosition;
						pos.y = -60.0f;
						cursor.GetComponent<RectTransform>().localPosition = pos;
					} break;
				case Menu.Exit: {
						var pos = cursor.GetComponent<RectTransform>().localPosition;
						pos.y = -120.0f;
						cursor.GetComponent<RectTransform>().localPosition = pos;
					} break;
			}
		}
	}
}