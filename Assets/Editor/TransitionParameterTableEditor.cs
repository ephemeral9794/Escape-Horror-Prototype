using UnityEngine;
using UnityEditor;
using EscapeHorror.Prototype;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

using Parameter = EscapeHorror.Prototype.TransitionParameterTable.Parameter;
[CustomEditor(typeof(TransitionParameterTable))]
public class TransitionParameterTableEditor : Editor
{
	public TransitionParameterTable Table => target as TransitionParameterTable;
	
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
		GUI.Label(rect, "Map Events");
	}
	private float GetElementHeight(int index)
	{
		return 50.0f;
	}
	private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
	{
		var param = Table.Parameters[index];
		float y = rect.yMin;
		EditorGUI.BeginChangeCheck();
		param.MapSceneNumber = EditorGUI.IntField(new Rect(rect.xMin, rect.yMin, rect.width, 16.0f), "Next Scene", param.MapSceneNumber);
		param.Position = EditorGUI.Vector2IntField(new Rect(rect.xMin, rect.yMin + 16.0f, rect.width, 16.0f), "Position", param.Position);
		param.Direction = EditorGUI.Vector2IntField(new Rect(rect.xMin, rect.yMin + 32.0f, rect.width, 16.0f), "Direction", param.Direction);
		Table.Parameters[index] = param;
		if (EditorGUI.EndChangeCheck())
		{
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
			MapSceneNumber = -1,
			Position = Vector2Int.zero,
			Direction = Vector2Int.zero
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
		if (EditorGUI.EndChangeCheck())
		{
			Save();
		}
		EditorGUILayout.Space();
		if (m_ReorderableList != null && Table.Parameters != null)
		{
			m_ReorderableList.DoLayoutList();
		}
	}
}