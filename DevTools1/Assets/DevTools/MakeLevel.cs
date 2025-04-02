using System;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class MakeLevel : EditorWindow
{
    private GameObject playerPrefab;
    private float width = 3;
    private float lenght = 3;
    private float height = 3;

    [MenuItem("Tools/Make Room")]
    public static void ShowWindow()
    {
        GetWindow<MakeLevel>("Make Room");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Room Maker", EditorStyles.boldLabel);

        playerPrefab = (GameObject)EditorGUILayout.ObjectField("Player", playerPrefab, typeof(GameObject), false);

        width = EditorGUILayout.FloatField("Width", width);
        lenght = EditorGUILayout.FloatField("Lenght", lenght);
        height = EditorGUILayout.FloatField("Height", height);

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(" Spawn Box Level", MessageType.Info);

        if (GUILayout.Button("Spawn Level"))
        {
            SpawnLevel();
        }
    }

    private void SpawnLevel()
    {
        GameObject parentObject = new GameObject("Level");

        if (playerPrefab != null)
        {
            GameObject newPlayer = Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            newPlayer.transform.SetParent(parentObject.transform);
        }
        else Debug.Log("There is no player");

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(width, cube.transform.localScale.y, lenght);
        cube.transform.SetParent(parentObject.transform);

        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.localScale = new Vector3(width, height, 0.1f);
            wall.transform.position = new Vector3(0, height / 2, lenght / 2);
            wall.transform.SetParent(parentObject.transform);
        }
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.localScale = new Vector3(width, height, 0.1f);
            wall.transform.position = new Vector3(0, height / 2, -lenght / 2);
            wall.transform.SetParent(parentObject.transform);
        }
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.localScale = new Vector3(0.1f, height, lenght);
            wall.transform.position = new Vector3(width / 2, height / 2, 0);
            wall.transform.SetParent(parentObject.transform);
        }
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.localScale = new Vector3(0.1f, height, lenght);
            wall.transform.position = new Vector3(-width / 2, height / 2, 0);
            wall.transform.SetParent(parentObject.transform);
        }

        Undo.RegisterCreatedObjectUndo(parentObject, "Make Simple Level");
    }
}
