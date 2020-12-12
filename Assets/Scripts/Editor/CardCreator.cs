using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardCreator : EditorWindow
{
    private const string UNDO_GAME_CARD_SPRITE = "Change Card Sprite";

    public string cardName = "NewCard";
    public Sprite cardSprite;
    [Range(1, 10)]
    public int cardLevel = 1;

    public CardPower powerUp = CardPower.Zero;
    public CardPower powerDown = CardPower.Zero;
    public CardPower powerLeft = CardPower.Zero;
    public CardPower powerRight = CardPower.Zero;

    private SerializedObject so;
    private SerializedProperty propCardName;
    private SerializedProperty propCardSprite;
    private SerializedProperty propCardLevel;

    private SerializedProperty propPowerUp;
    private SerializedProperty propPowerDown;
    private SerializedProperty propPowerLeft;
    private SerializedProperty propPowerRight;

    [MenuItem("Tools/Pony Triad/Card Creator")]
    private static void Init() {
        var window = GetWindow<CardCreator>("Card Creator");
        window.minSize = window.maxSize = new Vector2(350, 305);

    }

    private void OnEnable()
    {
        so = new SerializedObject(this);
        propCardName = so.FindProperty("cardName");
        propCardSprite = so.FindProperty("cardSprite");
        propCardLevel = so.FindProperty("cardLevel");

        propPowerUp = so.FindProperty("powerUp");
        propPowerDown = so.FindProperty("powerDown");
        propPowerLeft = so.FindProperty("powerLeft");
        propPowerRight = so.FindProperty("powerRight");
    }

    private void OnGUI()
    {
        // TODO : choisir le chemin de création de l'asset
        var labelStyle = new GUIStyle(GUI.skin.label) {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        EditorGUILayout.LabelField("Card Creator", labelStyle, GUILayout.ExpandWidth(true));

        EditorGUILayout.Space(10f);

        so.Update();
        EditorGUILayout.PropertyField(propCardName, new GUIContent("Name"));

        EditorGUI.BeginChangeCheck();
        propCardSprite.objectReferenceValue = EditorGUILayout.ObjectField("Icon", propCardSprite.objectReferenceValue, typeof(Sprite), false);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(propCardSprite.objectReferenceValue, UNDO_GAME_CARD_SPRITE);
        }

        EditorGUILayout.PropertyField(propCardLevel);

        EditorGUILayout.Space(10f);

        using(new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Power Values", EditorStyles.boldLabel);
            EditorGUIUtility.labelWidth = 50;
            EditorGUILayout.Space();

            var powerFieldLayoutWidth = GUILayout.Width(150);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(propPowerUp, new GUIContent("Up"), powerFieldLayoutWidth);
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space(10f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(propPowerLeft, new GUIContent("Left"), powerFieldLayoutWidth);
                EditorGUILayout.Space(10f);
                EditorGUILayout.PropertyField(propPowerRight, new GUIContent("Right"), powerFieldLayoutWidth);
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space(10f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(propPowerDown, new GUIContent("Down"), powerFieldLayoutWidth);
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space();
            EditorGUIUtility.labelWidth = 0;
        }
        so.ApplyModifiedProperties();

        EditorGUILayout.Space(10f);

        // Définir le niveau de la carte
        if (GUILayout.Button("Create Card"))
        {
            CardData newCard = CreateInstance<CardData>();
            newCard.spriteImage = cardSprite;
            newCard.powerUp = (CardPower)propPowerUp.enumValueIndex;
            newCard.powerDown = (CardPower)propPowerDown.enumValueIndex;
            newCard.powerLeft = (CardPower)propPowerLeft.enumValueIndex;
            newCard.powerRight = (CardPower)propPowerRight.enumValueIndex;

            AssetDatabase.CreateAsset(newCard, $"Assets/Datas/Cards/CardsByLevel/Level{propCardLevel.intValue}/{cardName}.asset");
        }
    }

    // Vérifier la validité les champs de la carte avant de la créer !
}
