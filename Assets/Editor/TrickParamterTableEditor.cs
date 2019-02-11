using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EscapeHorror.Prototype;
using UnityEditorInternal;

using Parameter = EscapeHorror.Prototype.TrickParameterTable.Parameter;
[CustomEditor(typeof(TrickParameterTable))]
public class TrickParamterTableEditor : Editor
{
	public TrickParameterTable Table => target as TrickParameterTable;

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
		param.ID = EditorGUI.IntField(new Rect(rect.xMin, rect.yMin, rect.width, 16.0f), "Parameter ID", param.ID);
		param.Type = (TrickParameterTable.TrickType)EditorGUI.EnumPopup(new Rect(rect.xMin, rect.yMin + 16.0f, rect.width, 16.0f), "Trick Type", param.Type);
		param.Position = EditorGUI.Vector2IntField(new Rect(rect.xMin, rect.yMin + 32.0f, rect.width, 16.0f), "Position", param.Position);
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
		Table.Parameters.Add(new Parameter
		{
			ID = -1,
			Position = Vector2Int.zero,
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