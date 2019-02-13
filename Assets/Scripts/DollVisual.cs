using UnityEngine;
using System.Collections.Generic;

namespace EscapeHorror.Prototype {
	[CreateAssetMenu(menuName = "Dolls/Doll Visual")]
	public class DollVisual : ScriptableObject
	{
		[SerializeField]
		public Sprite front;
		[SerializeField]
		public Sprite back;
		[SerializeField]
		public Sprite right;
		[SerializeField]
		public Sprite left;
	}
}