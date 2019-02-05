using UnityEngine;
using System.Collections.Generic;

namespace EscapeHorror.Prototype { 
	[CreateAssetMenu(menuName = "Scenario/Scenario Text")]
	public class ScenarioText : ScriptableObject
	{
		[SerializeField]
		string[] texts;
		public string[] Texts => texts;
	}
}
