using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Map Event Data", menuName = "Map Edit/Map Event Data")]
public class MapEventData : ScriptableObject {
	public enum Event {
		None,
		Talk,
	}
	public struct MapEvent{
		public Vector2Int Position;
		public Event Event;
	}

	[SerializeField]
	public List<MapEvent> m_MapEvents;
}
