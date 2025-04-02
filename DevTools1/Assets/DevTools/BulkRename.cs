using UnityEngine;
using UnityEditor;
using System;

public class BulkRename : EditorWindow
{
    private string baseName = "Name";
    private int startIndex = 0;
    private string suffix = "Suffix";

    [MenuItem("Tools/Bulk Renameer")]
    public static void ShowWindow()
    {
        GetWindow<BulkRename>("Bulk Renamer");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Bulk Renamer", EditorStyles.boldLabel);
        baseName = EditorGUILayout.TextField("Base Name", baseName);
        startIndex = EditorGUILayout.IntField("Start Index", startIndex);
        suffix = EditorGUILayout.TextField("Suffix", suffix);

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("Selsect the objects in the hierarcy, then click the button", MessageType.Info);

        if (GUILayout.Button("Rename Selected"))
        {
            RenameSelectedObjects();
        }
    }

    private void RenameSelectedObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            string newName = string.Format("{0}_{1}_{2}", baseName, (startIndex + i), suffix);

            Undo.RecordObject(selectedObjects[i], newName);

            selectedObjects[i].name = newName;
        }
    }
}
