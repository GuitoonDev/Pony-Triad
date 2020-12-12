using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardLevelData))]
public class CardLevelDataEditor : Editor
{
    private SerializedObject so;
    private SerializedProperty propLevel;
    private SerializedProperty propCardDataArray;

    private void OnEnable()
    {
        so = serializedObject;
        propLevel = so.FindProperty("level");
        propCardDataArray = so.FindProperty("cardDataArray");
    }

    public override void OnInspectorGUI()
    {
        so.Update();
        if (GUILayout.Button("Refresh"))
        {
            string[] cardDataGUIDArray =  AssetDatabase.FindAssets("t:CardData", new[] {$"Assets/Datas/Cards/CardsByLevel/Level{propLevel.intValue}" });
            propCardDataArray.arraySize = cardDataGUIDArray.Length;

            for (int i = 0; i < cardDataGUIDArray.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(cardDataGUIDArray[i]);
                propCardDataArray.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<CardData>(assetPath);
            }
        }
        so.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}
