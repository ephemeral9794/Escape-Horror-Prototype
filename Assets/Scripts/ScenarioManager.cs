using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeHorror.Prototype { 
	public class ScenarioManager : MonoBehaviour {
		[SerializeField]
		private ScenarioText scenario;
		[SerializeField]
		private TextAsset scenarioText;

		private TextController text;
		private CommandController command;
		private string rawScenarioText;

		public int Length => scenario.Texts.Length;
		public string this[int line]
		{
			get { return scenario.Texts[line]; }
		}

		private void Awake()
		{
			rawScenarioText = scenarioText.ToString();
			Debug.Log(text);
		}

		private void Start()
		{
			text = GetComponent<TextController>();
			command = GetComponent<CommandController>();
		}

		void AnalysisScenario()
		{
			int count = 0;
			foreach (var c in rawScenarioText) {
				if (c == '\n') {
					count++;
				}
			}
		}
	}
}
