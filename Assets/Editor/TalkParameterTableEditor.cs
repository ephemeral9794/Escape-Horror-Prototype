using UnityEngine;
using UnityEditor;
using EscapeHorror.Prototype;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

using Parameter = EscapeHorror.Prototype.TalkParameterTable.Parameter;
[CustomEditor(typeof(TalkParameterTable))]
public class TalkParameterTableEditor : Editor
{
	public TalkParameterTable Table => target as TalkParameterTable;
	
	private ReorderableList m_ReorderableList;

	private void OnEnable()
	{
		if (Table.Parameters == null)
		{
			Table.Parameters = new List<Parameter>();
		}

		m_ReorderableList = new ReorderableList(Table.Parameters, typeof(Parameter), true, true, true, true);
		m_ReorderableList.drawHeaderCallback += OnDrawHeader;
		m_ReorderableList.drawElementCallback += OnDrawElement;
		m_ReorderableList.elementHeightCallback += GetElementHeight;
		m_ReorderableList.onReorderCallback += ListUpdated;
		m_ReorderableList.onAddCallback += OnAddElement;

	}

	// ReorderableList Callbacks
	private void OnDrawHeader(Rect rect)
	{
		GUI.Label(rect, "Transition Parameter");
	}
	private float GetElementHeight(int index)
	{
		return 34.0f;
	}
	private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
	{
		var param = Table.Parameters[index];
		float y = rect.yMin;
		EditorGUI.BeginChangeCheck();
		param.ID = EditorGUI.IntField(new Rect(rect.xMin, rect.yMin, rect.width, 16.0f), "ID", param.ID);
		param.Scenario = (TextAsset)EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin + 16.0f, rect.width, 16.0f), "Scenario", param.Scenario, typeof(TextAsset), false);
		Table.Parameters[index] = param;
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
		Table.Parameters.Add(new Parameter {
			ID = -1,
			Scenario = null
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
		Table.CurrentScene = EditorGUILayout.IntField("Current Scene", Table.CurrentScene);
		if (EditorGUI.EndChangeCheck()) {
			Save();
		}
		EditorGUILayout.Space();
		if (m_ReorderableList != null && Table.Parameters != null) {
			m_ReorderableList.DoLayoutList();
		}
	}
}