using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;
using System.Text.RegularExpressions;
using DG.Tweening;

namespace EscapeHorror.Prototype { 
    [RequireComponent(typeof(AudioSource))]
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
		public TextAsset scenarioText;
		[SerializeField]
		int nextScene;
		[SerializeField]
		public bool overlay = false;

		private TextController text;
		private CommandController command;
		private CharacterVisualizer visualizer;
		private GameManager manager;
        private new AudioSource audio;
		private BackgroundPack backgroundPack;
        private SoundPack soundPack;
        private Image background;
        private Image overLayer;
		private CanvasGroup group;
		public bool IsEnabled{get; set;}
		private string[] TextLines;
		private List<Line> Lines;
        private Action<Unit> action;

		public int Length => TextLines.Length;
		public string this[int line]
		{
			get { return TextLines[line]; }
		}

		private void Awake()
		{
			backgroundPack = Resources.Load<BackgroundPack>("BackgroundPack");
            soundPack = Resources.Load<SoundPack>("SoundPack");
			IsEnabled = true;
		}

		private void Start()
		{
			manager = FindObjectOfType<GameManager>();
			text = GetComponent<TextController>();
			command = GetComponent<CommandController>();
			visualizer = GetComponentInChildren<CharacterVisualizer>();
            audio = GetComponent<AudioSource>();
            group = GetComponent<CanvasGroup>();
            background = transform.Find("Background").gameObject.GetComponent<Image>();
            overLayer = transform.Find("Overlay").gameObject.GetComponent<Image>();

            //var texts = scenarioText.ToString();
            if (scenarioText != null) { 
			    AnalysisScenario();
            }

			action = (_) => {
				if (IsEnabled) { 
					int current = text.CurrentLine;
					while (current < Lines.Count && command.IsCommand(current)) { 
						command.Execute(this, command[current]);
						current++;
					}
					text.CurrentLine = current;
					if (current < Lines.Count && !command.IsCommand(current)) {
						text.TextUpdate(Lines[current]);
					}
					if (current >= Lines.Count)
					{
						var tween = DOTween.To(() => group.alpha,
								  (a) => group.alpha = a,
								  0.0f, 1.0f);
						if (!overlay) {
							tween.OnComplete(() => manager.ChangeScene(nextScene));
						} else {
							tween.OnComplete(() => Disable());
						}
					}
				}
			};
			this.OnKeyDownAsObservable(KeyCode.Space).Subscribe(action).AddTo(this);
			this.OnKeyDownAsObservable(KeyCode.Return).Subscribe(action).AddTo(this);
			action.Invoke(new Unit());
		}

		public void Enable() {
			//group.alpha = 1;
            DOTween.To(() => group.alpha,
                       (a) => group.alpha = a,
                       1.0f, 1.0f)
                   .OnComplete(() => { IsEnabled = true; action.Invoke(new Unit()); });
            //IsEnabled = true;
		}
		public void Disable() {
			//group.alpha = 0;
			//IsEnabled = false;
            DOTween.To(() => group.alpha,
                       (a) => group.alpha = a,
                       0.0f, 1.0f)
                   .OnComplete(() => IsEnabled = false);
		}

		public void AnalysisScenario()
		{
			AnalysisScenario(scenarioText.ToString());
		}
		public void AnalysisScenario(string rawText)
		{
            //Debug.Log(rawText);
            command.ResetCommand();
            text.ResetText();
            visualizer.InvisibleAll();
			TextLines = rawText.Split('\n');
            if (Lines == null) { 
			    Lines = new List<Line>();
            } else {
                Lines.Clear();
            }

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
            //action.Invoke(new Unit());
        }

		public void ClearText() => text.ClearText();
		public void ChangeName(string name) => text.ChangeName(name);
		public void Visible(CharacterVisualizer.Position pos, string typeStr, int diffNum) => visualizer.Visible(pos, typeStr, diffNum);
		public void Invisible(CharacterVisualizer.Position pos) => visualizer.Invisible(pos);
		public void InvisibleAll() => visualizer.InvisibleAll();
		public void ChangeBackground(int num)
		{
			var pack = backgroundPack.PackSets[num];
			background.sprite = pack.sprite;
			background.color = pack.color;
		}
        public void Dawn()
        {
            overLayer.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        public void Darkness()
        {
            overLayer.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
        }
        public void Sound(int id)
        {
            var clip = soundPack.Sounds[id];
            audio.clip = clip;
            audio.Play();
        }
    }
}
