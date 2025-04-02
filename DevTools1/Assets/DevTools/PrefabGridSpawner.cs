using UnityEngine;
using UnityEditor;
using System;

public class PrefabGridSpawner : EditorWindow
{
    private GameObject PrefabToSpawn;
    private int rows = 3;
    private int cols = 3;
    private float horizontalSpacing = 2f;
    private float verticalSpacing = 2f;

    [MenuItem("Tools/Prefabs Grid Spawner")]
    public static void ShowWindow()
    {
        GetWindow<PrefabGridSpawner>("Grid Spawner");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Prefab Grid Spawner", EditorStyles.boldLabel);

        PrefabToSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab", PrefabToSpawn, typeof(GameObject), false);

        rows = EditorGUILayout.IntField("Rows", rows);
        cols = EditorGUILayout.IntField("Cols", cols);

        horizontalSpacing = EditorGUILayout.FloatField("Horizontal Spacing", horizontalSpacing);
        verticalSpacing = EditorGUILayout.FloatField("vertical Spacing ", verticalSpacing);

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(" Select a prefab, change the grid size, and press the button to create a grid", MessageType.Info);

        if (GUILayout.Button("Spawn Grid"))
        {
            SpawnGrid();
        }
    }

    private void SpawnGrid()
    {
        if (PrefabToSpawn == null)
        {
            Debug.LogWarning("No Prefab assigned");
            return;
        }

        GameObject parentObject = new GameObject("Prefab Grid");

        Undo.RegisterCreatedObjectUndo(parentObject, "Created GridParant");

        for (int r = 0; r < rows; r ++)
        {
            for (int c = 0; c < cols; c ++)
            {
                float posX = c * horizontalSpacing;
                float posY = r * verticalSpacing;

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(PrefabToSpawn);

                instance.transform.position = new Vector3(posX, 0, posY);
                instance.transform.SetParent(parentObject.transform);

                Undo.RegisterCreatedObjectUndo(parentObject, "Spawn Grid Element");
            }
        }
    }
}
