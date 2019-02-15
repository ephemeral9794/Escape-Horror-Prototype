using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EscapeHorror.Prototype { 
	[CreateAssetMenu(menuName = "Map Edit/Transition Parameter Table")]
	public class TransitionParameterTable : ScriptableObject
	{
		[Serializable]
		public struct Parameter
		{
			public int MapSceneNumber;
			public Vector2Int Position;
			public Vector2Int Direction;
		}

		[SerializeField]
		public int CurrentScene;
		[SerializeField]
		public List<Parameter> Parameters;

		public Parameter[] GetParameters(int SceneNumber)
		{
			return Parameters.Where(p => p.MapSceneNumber == SceneNumber).ToArray();
		}
	}
}
