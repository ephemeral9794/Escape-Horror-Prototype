using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using EscapeHorror.Prototype;
using System.Collections.Generic;

[CustomEditor(typeof(BackgroundPack))]
public class BackgroundPackEditor : Editor
{
	public BackgroundPack Pack => target as BackgroundPack;

	private ReorderableList m_ReorderableList;

	private void OnEnable()
	{
		if (Pack.PackSets == null) {
			Pack.PackSets = new List<BackgroundPack.PackSet>();
		}

		m_ReorderableList = new ReorderableList(Pack.PackSets, typeof(BackgroundPack.PackSet), true, true, true, true);
		m_ReorderableList.drawHeaderCallback += OnDrawHeader;
		m_ReorderableList.drawElementCallback += OnDrawElement;
		m_ReorderableList.elementHeightCallback += GetElementHeight;
		m_ReorderableList.onReorderCallback += ListUpdated;
		m_ReorderableList.onAddCallback += OnAddElement;
	}

	// ReorderableList Callbacks
	private void OnDrawHeader(Rect rect)
	{
		GUI.Label(rect, "Backgrounds");
	}
	private float GetElementHeight(int index)
	{
		return 34.0f;
	}
	private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
	{
		EditorGUI.BeginChangeCheck();
		var pack = Pack.PackSets[index];
		pack.color = EditorGUI.ColorField(new Rect(rect.xMin, rect.yMin, rect.width, 16.0f), "Color", pack.color);
		pack.sprite = (Sprite)EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin + 16.0f, rect.width, 16.0f), "Sprite", pack.sprite, typeof(Sprite), false);
		Pack.PackSets[index] = pack;
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
		Pack.PackSets.Add(new BackgroundPack.PackSet{
			color = new Color(),
			sprite = null,
		});
	}
	private void Save()
	{
		EditorUtility.SetDirty(target);
		AssetDatabase.SaveAssets();
	}

	public override void OnInspectorGUI()
	{
		if (m_ReorderableList != null && Pack.PackSets != null) {
			m_ReorderableList.DoLayoutList();
		}
	}
}