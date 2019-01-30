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
	
		private Image cursor;
		private GameManager manager;
		private Menu select = 0;
		private GameObject instance;
		private bool flag;

		// Use this for initialization
		void Start () {
			flag = true;
			cursor = GetComponentInChildren<Image>();
			manager = FindObjectOfType<GameManager>();
			this.OnKeyDownAsObservable(KeyCode.UpArrow).Subscribe(_ => {
				UpMenu();
			});
			this.OnKeyDownAsObservable(KeyCode.DownArrow).Subscribe(_ => {
				DownMenu();
			});
			this.OnKeyDownAsObservable(KeyCode.Space).Subscribe(_ => {
				switch (select)
				{
					case Menu.Start:
						manager.ChangeScene(1);
						break;
					case Menu.Desc:
						if (flag) { 
							instance = Instantiate(descBoard, transform.parent);
							instance.OnDestroyAsObservable().Subscribe(__ => flag = true);
							flag = false;
						}
						break;
					case Menu.Exit:
						Application.Quit();
						break;
				}
			});
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