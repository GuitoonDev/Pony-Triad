using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneAttribute))]
public class SceneDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string[] sceneNames = Array.ConvertAll(EditorBuildSettings.scenes, (EditorBuildSettingsScene scene) => {
            return Path.GetFileNameWithoutExtension(scene.path);
        });

        EditorGUI.BeginProperty(position, label, property);
        property.intValue = EditorGUI.Popup(position, label.text, property.intValue, sceneNames);
        EditorGUI.EndProperty();
    }
}
