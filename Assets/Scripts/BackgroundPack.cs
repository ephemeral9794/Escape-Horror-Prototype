using UnityEngine;
using System.Collections.Generic;
using System;

namespace EscapeHorror.Prototype { 
	[CreateAssetMenu(menuName = "Background Pack")]
	public class BackgroundPack : ScriptableObject
	{
		[Serializable]
		public struct PackSet {
			public Color color;
			public Sprite sprite;
		}

		[SerializeField]
		public List<PackSet> PackSets;
	}
}