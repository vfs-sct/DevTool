using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

public class SimpleExampleWindow : EditorWindow
{
    [MenuItem("Tools/Simple Example")]
    public static void ShowWindow()
    {
        var window = GetWindow<SimpleExampleWindow>();
        window.titleContent = new GUIContent("Example");
        window.Show();
    }

    private void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolKitStuff/SimpleExample.uxml");
        VisualElement content = visualTree.Instantiate();
        rootVisualElement.Add(content);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UIToolKitStuff/SimpleExampleStyleSheet.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        var label = rootVisualElement.Q<Label>("FirstLabel");
        var button = rootVisualElement.Q<Button>("FirstButton");

        button.clicked += () =>
        {
            label.text = "New text :D";
        };
    }
}