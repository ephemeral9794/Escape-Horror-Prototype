using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EscapeHorror.Prototype { 
	[Serializable]
	[CreateAssetMenu(fileName = "New Map Event Data", menuName = "Map Edit/Map Event Data")]
	public class MapEventData : ScriptableObject {
		public enum Event {
			None,
			Talk,
			Trick,
			Transition,
			Transition_Action,
		}
		/*[Serializable]
		public class IParameter { 
		
		}
		[Serializable]
		public class NoneParameter : IParameter { }
		[Serializable]
		public class TransitionParameter : IParameter
		{
			public int NextSceneNumber;
			public Vector2Int NextPosition;
			public Vector2Int NextDirection;
		}*/

		[Serializable]
		public struct MapEvent {
			public Vector2Int Position;
			public Event Event;
			public int NextScene;
		}

		[SerializeField]
		public int SceneNumber;
		[SerializeField]
		public List<MapEvent> m_MapEvents;

		public MapEvent this[Vector2Int pos] {
			get { return m_MapEvents.SingleOrDefault(value => (value.Position == pos));}
		}
	}
}