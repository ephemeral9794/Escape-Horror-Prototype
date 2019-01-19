using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(MapEventData))]
public class MapEventDataEditor : Editor
{
	public MapEventData Data => target as MapEventData;

	private ReorderableList m_ReorderableList;

	private void OnEnable()
	{
		if (Data.m_MapEvents == null) {
			Data.m_MapEvents = new List<MapEventData.MapEvent>();
		}

		m_ReorderableList = new ReorderableList(Data.m_MapEvents, typeof(MapEventData.MapEvent), true, true, true, true);
		m_ReorderableList.drawHeaderCallback += OnDrawHeader;
		m_ReorderableList.drawElementCallback += OnDrawElement;
		m_ReorderableList.elementHeightCallback += GetElementHeight;
		m_ReorderableList.onReorderCallback += ListUpdated;
		m_ReorderableList.onAddCallback += OnAddElement;
	}

	// ReorderableList Callbacks
	private void OnDrawHeader(Rect rect)
	{
		GUI.Label(rect, "Map Events");
	}
	private float GetElementHeight(int index)
	{
		return 34.0f;
	}
	private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
	{
		var mapEvent = Data.m_MapEvents[index];
		EditorGUI.BeginChangeCheck();
		float y = rect.yMin;
		mapEvent.Position = EditorGUI.Vector2IntField(new Rect(rect.xMin, rect.yMin, rect.width, 16.0f), "Position", Data.m_MapEvents[index].Position);
		mapEvent.Event = (MapEventData.Event)EditorGUI.EnumPopup(new Rect(rect.xMin, rect.yMin + 16.0f, rect.width, 16.0f), "Event", mapEvent.Event);
		Data.m_MapEvents[index] = mapEvent;
		if (EditorGUI.EndChangeCheck()) {
			Save();
		}
	}
	private void ListUpdated(ReorderableList list)
	{
		Save();
	}
	private void OnAddElement(ReorderableList list)
	{
		Data.m_MapEvents.Add(new MapEventData.MapEvent {
			Position = new Vector2Int(0, 0), 
			Event = MapEventData.Event.None
		});
	}
	private void Save()
	{
		EditorUtility.SetDirty(target);
		AssetDatabase.SaveAssets();
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		Data.SceneNumber = EditorGUILayout.IntField("Scene Number", Data.SceneNumber);
		EditorGUILayout.Space();
		if (m_ReorderableList != null && Data.m_MapEvents != null)
		{
			m_ReorderableList.DoLayoutList();
		}
		if (EditorGUI.EndChangeCheck())
		{
			Save();
		}
	}
}
