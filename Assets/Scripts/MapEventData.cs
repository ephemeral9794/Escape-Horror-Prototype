using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Map Event Data", menuName = "Map Edit/Map Event Data")]
public class MapEventData : ScriptableObject {
	public enum Event {
		None,
		Talk,
		Trick,
	}
	[Serializable]
	public struct MapEvent{
		public Vector2Int Position;
		public Event Event;
		public override string ToString()
		{
			return $"[{Position}, {Event}]";
		}
	}

	[SerializeField]
	public List<MapEvent> m_MapEvents;

	public MapEvent this[Vector2Int pos] {
		get { return m_MapEvents.SingleOrDefault(value => value.Position == pos);}
	}
}
