using UnityEngine;
using UnityEditor;
using EscapeHorror.Prototype;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(DollParameter))]
public class DollParameterEditor : Editor
{
	public enum Direct {
		Neutral, Up, Down, Right, Left
	}

	public DollParameter Param => target as DollParameter;

	private ReorderableList m_ReorderableList;

	private void OnEnable()
	{
		if (Param.Navigations == null)
		{
			Param.Navigations = new List<DollParameter.DollNavigation>();
		}

		m_ReorderableList = new ReorderableList(Param.Navigations, typeof(DollParameter.DollNavigation), true, true, true, true);
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
		var param = Param.Navigations[index];
		float y = rect.yMin;
		Direct direct = Direct.Neutral;
		if (param.Direct == Vector2Int.right) {
			direct = Direct.Right;
		} else if (param.Direct == Vector2Int.left) {
			direct = Direct.Left;
		} else if (param.Direct == Vector2Int.up) {
			direct = Direct.Up;
		} else if (param.Direct == Vector2Int.down) {
			direct = Direct.Down;
		}
		
		EditorGUI.BeginChangeCheck();
		param.Position = EditorGUI.Vector2IntField(new Rect(rect.xMin, rect.yMin, rect.width, 16.0f), "Position", param.Position);
		direct = (Direct)EditorGUI.EnumPopup(new Rect(rect.xMin, rect.yMin + 16.0f, rect.width, 16.0f), "Direction", direct);
		//param.Direct = EditorGUI.Vector2IntField(new Rect(rect.xMin, rect.yMin + 16.0f, rect.width, 16.0f), "Direction", param.Direct);
		switch (direct)
		{
			case Direct.Neutral:
				param.Direct = Vector2Int.zero;
				break;
			case Direct.Up:
				param.Direct = Vector2Int.up;
				break;
			case Direct.Down:
				param.Direct = Vector2Int.down;
				break;
			case Direct.Right:
				param.Direct = Vector2Int.right;
				break;
			case Direct.Left:
				param.Direct = Vector2Int.left;
				break;
		}

		Param.Navigations[index] = param;
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
		Param.Navigations.Add(new DollParameter.DollNavigation
		{
			Position = Vector2Int.zero,
			Direct = Vector2Int.zero
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
		Param.Visual = (DollVisual)EditorGUILayout.ObjectField("Visual", Param.Visual, typeof(DollVisual), false);
		if (EditorGUI.EndChangeCheck())
		{
			Save();
		}
		EditorGUILayout.Space();
		if (m_ReorderableList != null && Param.Navigations != null)
		{
			m_ReorderableList.DoLayoutList();
		}
	}
}