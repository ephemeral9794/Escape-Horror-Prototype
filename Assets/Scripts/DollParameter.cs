using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EscapeHorror.Prototype
{
	[CreateAssetMenu(menuName = "Dolls/Doll Parameter")]
	public class DollParameter : ScriptableObject
	{
		[Serializable]
		public struct DollNavigation
		{
			public Vector2Int Position;
			public Vector2Int Direct;	// 次のポジションへの向き
		}

		[SerializeField]
		public DollVisual Visual;
		[SerializeField]
		public List<DollNavigation> Navigations;
	}
}
