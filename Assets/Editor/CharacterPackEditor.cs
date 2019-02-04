using UnityEngine;
using UnityEditor;
using EscapeHorror.Prototype;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(CharacterPack))]
public class CharacterPackEditor : Editor
{
    public CharacterPack Data => target as CharacterPack;

    private ReorderableList m_ReorderableList;

    private void OnEnable()
    {
        if (Data.Sprites == null)
        {
            Data.Sprites = new List<Sprite>();
        }

        m_ReorderableList = new ReorderableList(Data.Sprites, typeof(Sprite), true, true, true, true);
        m_ReorderableList.drawHeaderCallback += OnDrawHeader;
        m_ReorderableList.drawElementCallback += OnDrawElement;
        m_ReorderableList.elementHeightCallback += GetElementHeight;
        m_ReorderableList.onReorderCallback += ListUpdated;
        m_ReorderableList.onAddCallback += OnAddElement;
    }

    // ReorderableList Callbacks
    private void OnDrawHeader(Rect rect)
    {
        GUI.Label(rect, "Sprites");
    }
    private float GetElementHeight(int index)
    {
        return 16.0f;
    }
    private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
    {
        EditorGUI.BeginChangeCheck();
        Data.Sprites[index] = (Sprite)EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin, rect.width, 16.0f), Data.Sprites[index], typeof(Sprite), false);
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
        Data.Sprites.Add(null);
    }
    private void Save()
    {
        /*foreach (var sprite in Data.Sprites) { 
            AssetDatabase.AddObjectToAsset(sprite, Data);
        }*/
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        Data.Character = (CharacterPack.CharacterType)EditorGUILayout.EnumPopup("Character", Data.Character);
        Data.Size = EditorGUILayout.RectField("Rectangle", Data.Size);
        if (EditorGUI.EndChangeCheck())
        {
            Save();
        }
        EditorGUILayout.Space();
        if (m_ReorderableList != null && Data.Sprites != null)
        {
            m_ReorderableList.DoLayoutList();
        }
    }
}