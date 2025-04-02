using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System;
public class AssetManagerWindow : EditorWindow
{
    private ListView duplicatesList;
    private ListView largeFilesList;
    private ListView unusedList;
    private ListView prefabsList;

    private List<string> duplicates = new List<string>();
    private List<string> largeFiles = new List<string>();
    private List<string> unused = new List<string>();
    private List<string> prefabs = new List<string>();

    [MenuItem("Tools/ Asset manager")]
    public static void ShowWndow()
    {
        var window = GetWindow<AssetManagerWindow>();
        window.titleContent = new GUIContent("Asset Manager");
        window.Show();
    }

    private void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolKitStuff/AssetManager.uxml");

        VisualElement content = visualTree.Instantiate();
        rootVisualElement.Add(content);

        Button scanButton = rootVisualElement.Q<Button>("scanButton");
        duplicatesList = rootVisualElement.Q<ListView>("duplicatesList");
        largeFilesList = rootVisualElement.Q<ListView>("largeFilesList");
        unusedList = rootVisualElement.Q<ListView>("unusedList");
        prefabsList = rootVisualElement.Q<ListView>("prefabsList");

        SetupListView(duplicatesList, duplicates);
        SetupListView(largeFilesList, largeFiles);
        SetupListView(unusedList, unused);
        SetupListView(prefabsList, prefabs);

        scanButton.clicked += () =>
        {
            RunScan();

            duplicatesList.Rebuild();
            largeFilesList.Rebuild();
            unusedList.Rebuild();
            prefabsList.Rebuild();
        };
    }

    private void SetupListView(ListView listView, List<string> dataSource)
    {
        listView.itemsSource = dataSource;

        listView.makeItem = () => new Label();
        listView.bindItem = (element, i) =>
        {
            var label = (Label)element;
            label.text = dataSource[i];
        };

        listView.fixedItemHeight = 16;
    }

    private void RunScan()
    {
        duplicates.Clear();
        largeFiles.Clear();
        unused.Clear();
        prefabs.Clear();

        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        var filterdAssets = allAssetPaths.Where(path => path.StartsWith("Assets/") && !Directory.Exists(path)).ToArray();

        // duplicates
        var hashToPath = new Dictionary<string, string>();
        foreach (var path in filterdAssets)
        {
            string fileHash = MD5Hash(path);
            if (hashToPath.ContainsKey(fileHash))
            {
                duplicates.Add($"{path} is duplicate of {hashToPath[fileHash]}");
            }
            else
            {
                hashToPath[fileHash] = path;
            }
        }

        // Large files
        foreach (var path in filterdAssets)
        {
            var fileInfo = new FileInfo(path);
            long sizeInBytes = fileInfo.Length;
            if (sizeInBytes > 10000)
            {
                largeFiles.Add($"{path} ({(sizeInBytes / 1024f / 1024):F2} MB)");
            }
        }

        // Unused assets
        var usedAssets = new HashSet<string>();

        var scenesInBuild = EditorBuildSettings.scenes
            .Where(s=> s.enabled)
            .Select(s => s.path);
        foreach (var scenePath in scenesInBuild)
        {
            var sceneDeps = AssetDatabase.GetDependencies(scenePath, true);
            foreach (var dep in sceneDeps)
            {
                usedAssets.Add(dep);
            }
        }

        foreach (var path in filterdAssets)
        {
            if (!usedAssets.Contains(path))
            {
                unused.Add(path);
            }
        }

        // Find All prefabs
        foreach (var path in filterdAssets)
        {
            if (Path.GetExtension(path).Equals(".prefab", System.StringComparison.OrdinalIgnoreCase))
            {
                prefabs.Add(path);
            }
        }

        duplicates.Sort();
        largeFiles.Sort();
        unused.Sort();
        prefabs.Sort(); 
    }

    private string MD5Hash(string path)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(path))
            {
                var hash = md5.ComputeHash(stream);
                return System.BitConverter.ToString(hash).Replace("-","").ToLowerInvariant();
            }
        }
    }
}
