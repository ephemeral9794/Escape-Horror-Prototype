using UnityEngine;
using UnityEditor;
using EscapeHorror.Prototype;
using UnityEngine.Audio;

[CustomEditor(typeof(ConfigPrefs))]
public class ConfigPrefsEditor : Editor
{
	ConfigPrefs Prefs => target as ConfigPrefs;

	private void Save()
	{
		Prefs.Apply();
		EditorUtility.SetDirty(target);
		AssetDatabase.SaveAssets();
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUI.BeginDisabledGroup(Prefs.Mixer != null);
		Prefs.Mixer = (AudioMixer)EditorGUILayout.ObjectField("Mixer", Prefs.Mixer, typeof(AudioMixer), false);
		EditorGUI.EndDisabledGroup();
		Prefs.MasterVolume = EditorGUILayout.Slider("Master Volume", Prefs.MasterVolume, 0.0f, 1.0f);
		Prefs.BGMVolume = EditorGUILayout.Slider("BGM Volume", Prefs.BGMVolume, 0.0f, 1.0f);
		Prefs.SEVolume = EditorGUILayout.Slider("SE Volume", Prefs.SEVolume, 0.0f, 1.0f);
		if (EditorGUI.EndChangeCheck()) {
			Save();
		}
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Apply", GUILayout.Width(48))) {
			Prefs.Apply();
			Save();
		}
		EditorGUILayout.EndHorizontal();
	}
}