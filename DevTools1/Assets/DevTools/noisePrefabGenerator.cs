using System;
using UnityEditor;
using UnityEngine;

public class noisePrefabGenerator : EditorWindow
{
    private GameObject prefabToSpawn;
    private int width = 50;
    private int height = 50;
    private float cellSize = 1f;
    private float noiseScale = 0.1f;
    private float threshold = 0.5f;
    private int seed = 0;

    private bool randomRotation = true;
    private float minScale = 1f;
    private float maxScale = 1f;

    [MenuItem("Tools/Noise Prefab Generator")]
    private static void ShowWindow()
    {
        GetWindow<noisePrefabGenerator>("Noise Prefab Generator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Noise-Based Prefab Spawner", EditorStyles.boldLabel);

        prefabToSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab", prefabToSpawn, typeof(GameObject), false);

        width = EditorGUILayout.IntField("Width (X Cells)", width);
        height = EditorGUILayout.IntField("Height (X Cells)", height);
        cellSize = EditorGUILayout.FloatField("Cell Size", cellSize);

        noiseScale = EditorGUILayout.FloatField("Noise Scalse", noiseScale);
        threshold = EditorGUILayout.Slider("Noise Thresshold", threshold, 0f, 1f);

        seed = EditorGUILayout.IntField("Seed", seed);

        randomRotation = EditorGUILayout.Toggle("Random Rotation", randomRotation);
        minScale = EditorGUILayout.FloatField("Min Scale", minScale);
        maxScale = EditorGUILayout.FloatField("Max Scale", maxScale);

        EditorGUILayout.Space();

        if (GUILayout.Button("SpawnObject"))
        {
            SpawnWithNoise();
        }
    }

    private void SpawnWithNoise()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab");
            return;
        }

        UnityEngine.Random.InitState(seed);

        GameObject parantObject = new GameObject("NoiseSpawnedObjects");

        Undo.RegisterCreatedObjectUndo(parantObject, "Create Noise Spawner Parant");

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float noiseValue = Mathf.PerlinNoise(x * noiseScale, z * noiseScale);

                if (noiseValue > threshold)
                {
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn);

                    Vector3 position = new Vector3(x * cellSize, 0f, z * cellSize);

                    instance.transform.position = position;

                    if (randomRotation)
                    {
                        float randomRot = UnityEngine.Random.Range(0f, 360f);
                        instance.transform.rotation = Quaternion.Euler(0f, randomRot, 0f);
                    }

                    if (maxScale >= minScale)
                    {
                        float scaleFactor = UnityEngine.Random.Range(minScale, maxScale);
                        instance.transform.localScale = Vector3.one * scaleFactor;
                    }
                    instance.transform.SetParent(parantObject.transform);

                    Undo.RegisterCreatedObjectUndo(instance, "Create Noise Spawner Parant");
                }
            }
        }
    }
}
