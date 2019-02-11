using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EscapeHorror.Prototype { 
	[CreateAssetMenu(menuName = "Map Edit/Trick Paramter Table")]
	public class TrickParameterTable : ScriptableObject
	{
		public enum TrickType
		{
			Doll,Remove,
		}
		[Serializable]
		public struct Parameter
		{
			public int ID;
			public TrickType Type;
			public Vector2Int Position;
		}

		[SerializeField]
		public int CurrentScene;
		[SerializeField]
		public List<Parameter> Parameters;

		public Parameter[] GetParameters(int id)
		{
			return Parameters.Where(p => p.ID == id).ToArray();
		}
	}
}
