using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockLibrary))]
public class BlockLibraryEditor : Editor
{
    private SerializedProperty dataProperty;

    private void OnEnable()
    {
        dataProperty = serializedObject.FindProperty("data");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        if(dataProperty.arraySize != byte.MaxValue) dataProperty.arraySize = byte.MaxValue;
        serializedObject.ApplyModifiedProperties();
    }
}
