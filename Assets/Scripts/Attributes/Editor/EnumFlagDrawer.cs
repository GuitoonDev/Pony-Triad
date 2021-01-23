using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var targetEnum = (Enum)Enum.ToObject(fieldInfo.FieldType, property.intValue);

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();
        Enum enumNew = EditorGUI.EnumFlagsField(position, label, targetEnum);
        if (EditorGUI.EndChangeCheck())
        {
            property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
        }
        EditorGUI.EndProperty();
    }
}