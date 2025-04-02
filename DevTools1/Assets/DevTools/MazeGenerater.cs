using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
//using System;

public class MazeGenerater : EditorWindow
{
    private int width = 50;
    private int height = 50;
    private float cellSize = 1f;
    private int seed = 0;

    public enum GenerationAlgorithm { CellularAutomata, PerlinNoise }
    private GenerationAlgorithm algorithm = GenerationAlgorithm.CellularAutomata;

    // Cellular Automata
    private int initialWallPercentage = 50;
    private int iterations = 5;

    // Perlin
    private float threshold = 0.5f;

    private bool applyFloodFill = true;

    private bool showCellularAutomataParams = true;
    private bool showPerlinNoiseParams = false;

    [MenuItem("Tools/Maze Generator")]
    public static void SuperCole()
    {
        GetWindow<MazeGenerater>("Improved Maze Generater");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("MazeParameters", EditorStyles.boldLabel);

        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        cellSize = EditorGUILayout.FloatField("Cell Size", cellSize);
        seed = EditorGUILayout.IntField("Seed", seed);

        algorithm = (GenerationAlgorithm)EditorGUILayout.EnumPopup("Algorithm", algorithm);

        applyFloodFill = EditorGUILayout.Toggle("Apply Flood Fill", applyFloodFill);

        showCellularAutomataParams = (algorithm == GenerationAlgorithm.CellularAutomata);
        showPerlinNoiseParams = (algorithm == GenerationAlgorithm.PerlinNoise);

        if (showCellularAutomataParams)
        {
            EditorGUILayout.LabelField("Cellular Automata Paramaters", EditorStyles.boldLabel);

            initialWallPercentage = EditorGUILayout.IntSlider("Initial Wall %", initialWallPercentage, 0, 100);
            iterations = EditorGUILayout.IntField("Iterations", iterations);
        }

        if (showPerlinNoiseParams)
        {
            EditorGUILayout.LabelField("Perlin Noise Parameters", EditorStyles.boldLabel);

            threshold = EditorGUILayout.Slider("Threshold", threshold, 0, 1f);
        }

        if (GUILayout.Button("Generate Maze"))
        {
            GenerateMaze();
        }
    }

    private void GenerateMaze()
    {
        GameObject mazeParant = new GameObject("Maze");

        Undo.RegisterCreatedObjectUndo(mazeParant, "Create Maze");

        switch (algorithm)
        {
            case GenerationAlgorithm.CellularAutomata:
                GenerateCellularAutomataMaze(mazeParant);
                break;
            case GenerationAlgorithm.PerlinNoise:
                GeneratePerlinNoiseMaze(mazeParant);
                break;
        }
    }

    private void GeneratePerlinNoiseMaze(GameObject mazeParant)
    {
        Random.InitState(seed);

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                float noiseValue = Mathf.PerlinNoise((x + seed) * 0.1f,(y + seed) * 0.1f);

                if (noiseValue > threshold)
                {
                    CreateWallCube(mazeParant, x, y);
                }
            }
        }
    }

    private void GenerateCellularAutomataMaze(GameObject mazeParant)
    {
        int[,] maze = new int[width, height];

        Random.InitState(seed);

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                int randValue = Random.Range(0, 100);
                int isWall = randValue < initialWallPercentage ? 1 : 0;
                maze[x, y] = isWall;
            }
        }

        for (int i = 0; i < iterations; i++)
        {
            maze = ApplyCellularAutomataRules(maze);
        }

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                if (maze[x,y] == 1)
                {
                    CreateWallCube(mazeParant, x, y);
                }
            }
        }
    }

    private void CreateWallCube(GameObject mazeParant, int x, int y)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(x * cellSize, 0, y * cellSize); 
        cube.transform.localScale = new Vector3(cellSize, 1f, cellSize);
        cube.transform.parent = mazeParant.transform;

        Undo.RegisterCreatedObjectUndo(cube, "Create Maze Cube");
    }

    private int[,] ApplyCellularAutomataRules(int[,] maze)
    {
        int[,] newMaze = new int[width, height];

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                int wallNeighbours = CountWallNeighbours(maze, x, y);

                if (maze[x,y]==1)
                {
                    newMaze[x, y] = wallNeighbours < 4 ? 0 : 1;
                }
                else
                {
                    newMaze[x, y] = wallNeighbours >= 5 ? 1 : 0;
                }
            }
        }
        return newMaze;
    }

    private int CountWallNeighbours(int[,] maze, int x, int y)
    {
        int count = 0;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < width && j >= 0 && j < height && !(i == x && j == y))
                {
                    count += maze[i, j];
                }
            }
        }
        return count;
    }
}
