using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardLevelData))]
public class CardLevelDataEditor : Editor
{
    private SerializedProperty propLevel;
    private SerializedProperty propCardDataArray;

    private void OnEnable()
    {
        propLevel = serializedObject.FindProperty("level");
        propCardDataArray = serializedObject.FindProperty("cardDataArray");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty nextProperty = serializedObject.GetIterator();
        while (nextProperty.NextVisible(!nextProperty.isArray))
        {
            EditorGUILayout.PropertyField(nextProperty);
            if(nextProperty.displayName.Equals("Script"))
            {
                if (GUILayout.Button("Refresh"))
                {
                    string[] cardDataGUIDArray = AssetDatabase.FindAssets("t:CardData", new[] { $"Assets/Datas/Cards/CardsByLevel/Level{propLevel.intValue}" });
                    propCardDataArray.arraySize = cardDataGUIDArray.Length;

                    for (int i = 0; i < cardDataGUIDArray.Length; i++)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(cardDataGUIDArray[i]);
                        propCardDataArray.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<CardData>(assetPath);
                    }
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
