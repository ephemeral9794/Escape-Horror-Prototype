using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;

namespace EscapeHorror.Prototype { 
	public class TextController : MonoBehaviour {
		/*[SerializeField]
		private string[] scenario;*/
		[SerializeField]
		private TextMeshProUGUI nameText;
		[SerializeField]
		private TextMeshProUGUI text;

		private CharacterVisualizer visualizer;
		private ScenarioManager scenario;
		int currentLine = 0; // 現在の行番号

		void Start()
		{
			visualizer = GetComponentInChildren<CharacterVisualizer>();
			scenario = GetComponent<ScenarioManager>();
			//Debug.Log(visualizer.gameObject.name);
			TextUpdate();
			Action<Unit> action =(_) => {
				if (currentLine < scenario.Length) {
					TextUpdate();
				}
			};
			this.OnKeyDownAsObservable(KeyCode.Space).Subscribe(action).AddTo(this);
			this.OnKeyDownAsObservable(KeyCode.Return).Subscribe(action).AddTo(this);
		}

		void Update()
		{
			// 現在の行番号がラストまで行ってない状態でクリックすると、テキストを更新する
			/*if (currentLine < scenario.Length && Input.GetMouseButtonDown(0))
			{
				TextUpdate();
			}*/
		}

		// テキストを更新する
		void TextUpdate()
		{
			// 現在の行のテキストをuiTextに流し込み、現在の行番号を一つ追加する
			text.text = scenario[currentLine];
			currentLine++;
		}
	}
}