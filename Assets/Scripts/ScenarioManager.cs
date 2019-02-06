using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Text.RegularExpressions;

namespace EscapeHorror.Prototype { 
	public class ScenarioManager : MonoBehaviour {
		public enum LineType{
			NormalText, WaitText, Command,
		}
		public struct Line
		{
			public LineType Type;
			public int LineCount;
			public string Text;
		}

		[SerializeField]
		private TextAsset scenarioText;

		private TextController text;
		private CommandController command;
		private CharacterVisualizer visualizer;
		private string[] TextLines;
		private List<Line> Lines;

		public int Length => TextLines.Length;
		public string this[int line]
		{
			get { return TextLines[line]; }
		}
		
		private void Start()
		{
			text = GetComponent<TextController>();
			command = GetComponent<CommandController>();
			visualizer = GetComponentInChildren<CharacterVisualizer>();

			var texts = scenarioText.ToString();
			AnalysisScenario(texts);

			Action<Unit> action = (_) => {
				int current = text.CurrentLine;
				while (current < Lines.Count && command.IsCommand(current)) { 
					command.Execute(this, command[current]);
					current++;
				}
				text.CurrentLine = current;
				if (current < Lines.Count && !command.IsCommand(current)) {
					text.TextUpdate(Lines[current]);
				}
			};
			this.OnKeyDownAsObservable(KeyCode.Space).Subscribe(action).AddTo(this);
			this.OnKeyDownAsObservable(KeyCode.Return).Subscribe(action).AddTo(this);
			action.Invoke(new Unit());
		}

		void AnalysisScenario(string rawText)
		{
			TextLines = rawText.Split('\n');
			Lines = new List<Line>();

			var regex = new Regex(@"\[.*]", RegexOptions.Compiled);
			//Debug.Log(regex);
			for (int i = 0; i < TextLines.Length; i++) {
				// 空行はスキップ
				if (string.IsNullOrEmpty(TextLines[i])) {
					continue;
				}

				var line = new Line {
					Type = LineType.NormalText,
					LineCount = i,
					Text = TextLines[i]
				};
				if (regex.IsMatch(line.Text)) {
					line.Type = LineType.Command;
					var commands = CommandController.Analysis(line);
					if (commands.tag == "br") {
						line.Type = LineType.WaitText;
						line.Text = line.Text.Replace("[br]", "\n"); 
					} else { 
						command.Add(commands);
					}
				}
				Lines.Add(line);
			}
		}

		public void ClearText() => text.ClearText();
		public void ChangeName(string name) => text.ChangeName(name);
	}
}
