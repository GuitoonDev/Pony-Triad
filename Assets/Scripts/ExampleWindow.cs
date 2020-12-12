using UnityEditor;
using UnityEngine;

public class ExampleWindow : EditorWindow
{
    [Range(0f,1f)]
    public float sliderValue = 0;
    public string labelText = "-";

    SerializedObject so;
    SerializedProperty propSliderValue;

    [MenuItem("Window/Example Window")]
    static void Init() => GetWindow(typeof(ExampleWindow));

    private void OnEnable()
    {
        so = new SerializedObject(this);
        propSliderValue = so.FindProperty("sliderValue");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("New value", labelText);

        // Start a code block to check for GUI changes

        //EditorGUI.BeginChangeCheck();

        so.Update();
        EditorGUILayout.PropertyField(propSliderValue);
        // End the code block and update the label if a change occurred
        //if (EditorGUI.EndChangeCheck())
        if (so.ApplyModifiedProperties())
        {
            labelText = propSliderValue.floatValue.ToString();
        }
    }
}