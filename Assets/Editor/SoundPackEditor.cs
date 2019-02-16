using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using EscapeHorror.Prototype;
using System.Collections.Generic;

[CustomEditor(typeof(SoundPack))]
public class SoundPackEditor : Editor
{
	public SoundPack Pack => target as SoundPack;

	private ReorderableList m_ReorderableList;

	private void OnEnable()
	{
		if (Pack.Sounds == null) {
			Pack.Sounds = new List<AudioClip>();
		}

		m_ReorderableList = new ReorderableList(Pack.Sounds, typeof(AudioClip), true, true, true, true);
		m_ReorderableList.drawHeaderCallback += OnDrawHeader;
		m_ReorderableList.drawElementCallback += OnDrawElement;
		m_ReorderableList.elementHeightCallback += GetElementHeight;
		m_ReorderableList.onReorderCallback += ListUpdated;
		m_ReorderableList.onAddCallback += OnAddElement;
	}

	// ReorderableList Callbacks
	private void OnDrawHeader(Rect rect)
	{
		GUI.Label(rect, "Sounds");
	}
	private float GetElementHeight(int index)
	{
		return 18.0f;
	}
	private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
	{
		EditorGUI.BeginChangeCheck();
        Pack.Sounds[index] = (AudioClip)EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin, rect.width, 16.0f), "Sound", Pack.Sounds[index], typeof(AudioClip), false);
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
		Pack.Sounds.Add(null);
	}
	private void Save()
	{
		EditorUtility.SetDirty(target);
		AssetDatabase.SaveAssets();
	}

	public override void OnInspectorGUI()
	{
		if (m_ReorderableList != null && Pack.Sounds != null) {
			m_ReorderableList.DoLayoutList();
		}
	}
}