using UnityEngine;
using UnityEditor;

public class IntroDevTools : EditorWindow
{
    private string fields1String = "Super Cole";
    private bool toggle1 = false;
    private float slider1 = 0.5f;

    [MenuItem("Tools/Intro Tool")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(IntroDevTools), false, "My Intro Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("This is the name", EditorStyles.boldLabel);
        fields1String = EditorGUILayout.TextField("Name:", fields1String);
        toggle1 = EditorGUILayout.Toggle("Toggle",toggle1);
        slider1 = EditorGUILayout.Slider("Slider", slider1 , 0f, 1f);
        if (GUILayout.Button("Press this :)")) Debug.Log("Button Pressed");
    }
}
