using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EscapeHorror.Prototype { 
	public struct Command
	{
		public string tag;
		public int line;
		public Dictionary<string, string> param;
	}
	public class CommandController : MonoBehaviour {
		/*private static readonly string[] CommandTags =
		{
			"[cm]", "[name]", "[char]"
		};*/

		private List<Command> commands = new List<Command>();

		public void Add(Command command) {
			commands.Add(command);
		}
        public void ResetCommand()
        {
            commands.Clear();
        }

        public static Command Analysis(ScenarioManager.Line line)
		{
			var command = new Command();
			
			// コマンド出現行を記録
			command.line = line.LineCount;
			
			// コマンド名を取得
			var tag = Regex.Match(line.Text, @"\[.*]");
			var tagstr = tag.Groups[0].ToString();
			command.tag = AnalysisTag(tagstr);
			//Debug.Log(tagstr);
			
			// コマンドのパラメータを取得
			Regex regex = new Regex("(\\S+)=(\\S+)");
			var matches = regex.Matches(tagstr);
			command.param = new Dictionary<string, string>();
			//Debug.Log(matches.Count);
			foreach (Match match in matches)
			{
				//Debug.LogFormat("{0},{1}", match.Groups[1].ToString(), match.Groups[2].ToString().TrimEnd(' ', ']'));
				command.param.Add(match.Groups[1].ToString(),match.Groups[2].ToString().TrimEnd(' ', ']'));
			}
			return command;
		}

		private static string AnalysisTag(string tag)
		{
			StringBuilder builder = new StringBuilder();
			bool flag = false;
			foreach (var s in tag)
			{
				if (s == '[')
				{
					flag = true;
					continue;
				} else if (s == ' ' || s == ']')
				{
					flag = false;
					break;
				}
				if (flag)
				{
					builder.Append(s);
				}
			}
			return builder.ToString();
		}
		
		public bool IsCommand(int line)
		{
			foreach (var command in commands)
			{
				if (command.line == line)
				{
					return true;
				}
			}
			return false;
		}
		public Command this[int n]
		{
			get { return commands.SingleOrDefault(c => c.line == n); }
		}

		public void Execute(ScenarioManager scenario, Command command)
		{
			//Debug.Log(command.tag);
			switch (command.tag)
			{
				case "cm": { 
						scenario.ClearText();
					} break;
				case "name": { 
						string name = command.param["name"];
						scenario.ChangeName(name);
					} break;
				case "char": { 
						int pos = int.Parse(command.param["pos"]);
						string type = command.param["name"];
						int diff = int.Parse(command.param["diff"]);
						scenario.Visible((CharacterVisualizer.Position)pos, type, diff);
					} break;
				case "unchar": {
						int pos = int.Parse(command.param["pos"]);
						scenario.Invisible((CharacterVisualizer.Position)pos);
					} break;
				case "reset" : {
						scenario.InvisibleAll();
					} break;
				case "back" : {
						int id = int.Parse(command.param["id"]);
						scenario.ChangeBackground(id);
					} break;
                case "dawn" : { 
                        scenario.Dawn();
                    } break;
                case "dark": {
                        scenario.Darkness();
                    } break;
                case "sound": {
                        int id = int.Parse(command.param["id"]);
                        scenario.Sound(id);
                    } break;
            }
		}
	}
}
