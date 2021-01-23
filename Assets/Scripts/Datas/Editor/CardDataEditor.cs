using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor
{
    private SerializedObject so;
    private SerializedProperty propSpriteImage;
    private SerializedProperty propPowerUp;
    private SerializedProperty propPowerDown;
    private SerializedProperty propPowerLeft;
    private SerializedProperty propPowerRight;

    private void OnEnable()
    {
        so = serializedObject;
        propSpriteImage = so.FindProperty("spriteImage");

        propPowerUp = so.FindProperty("powerUp");
        propPowerDown = so.FindProperty("powerDown");
        propPowerLeft = so.FindProperty("powerLeft");
        propPowerRight = so.FindProperty("powerRight");
    }

    public override void OnInspectorGUI()
    {
        so.Update();
        propSpriteImage.objectReferenceValue = EditorGUILayout.ObjectField("Icon", propSpriteImage.objectReferenceValue, typeof(Sprite), false);

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Power Values", EditorStyles.boldLabel);

            var powerFieldLayoutWidth = GUILayout.Width(100);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(propPowerUp, new GUIContent(""), powerFieldLayoutWidth);
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(propPowerLeft, new GUIContent(""), powerFieldLayoutWidth);
                EditorGUILayout.Space(10f);
                EditorGUILayout.PropertyField(propPowerRight, new GUIContent(""), powerFieldLayoutWidth);
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(propPowerDown, new GUIContent(""), powerFieldLayoutWidth);
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space();
        }
        so.ApplyModifiedProperties();
    }
}
