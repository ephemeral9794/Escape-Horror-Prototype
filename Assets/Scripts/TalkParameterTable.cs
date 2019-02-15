using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EscapeHorror.Prototype {
	[CreateAssetMenu(menuName = "Map Edit/Talk Parameter Table")]
	public class TalkParameterTable : ScriptableObject
	{
		[Serializable]
		public struct Parameter {
			public int ID;
			public TextAsset Scenario;
		}

		[SerializeField]
		public int CurrentScene;
		[SerializeField]
		public List<Parameter> Parameters;

		public Parameter[] GetParameters(int SceneNumber)
		{
			return Parameters.Where(p => p.ID == SceneNumber).ToArray();
		}
	}
}