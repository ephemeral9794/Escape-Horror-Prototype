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

		//private CharacterVisualizer visualizer;
		//private ScenarioManager scenario;
		public int CurrentLine { get; set; } // 現在の行番号
		
		private void Awake()
		{
			CurrentLine = 0;
		}

		public void TextUpdate(ScenarioManager.Line line)
		{
			switch (line.Type)
			{
				case ScenarioManager.LineType.NormalText:
				case ScenarioManager.LineType.WaitText:
					string t = text.text;
					t += line.Text;
					text.text = t;
					break;
			}
			CurrentLine++;
		}
        public void ResetText()
        {
            ClearText();
            CurrentLine = 0;
        }

		public void ClearText()
		{
			text.text = "";
		}
		public void ChangeName(string name)
		{
			nameText.text = name;
		}
	}
}