using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Cards List")]
    [SerializeField] private CardList cardsList = null;

    [Header("Player One")]
    [SerializeField] private TextMeshPro playerOneScore = null;
    [SerializeField] private CardHand playerOneCardsHolder = null;

    [Header("Player Two")]
    [SerializeField] private TextMeshPro playerTwoScore = null;
    [SerializeField] private CardHand playerTwoCardsHolder = null;

    [Header("Field Areas")]
    [SerializeField] private List<VerticalListableAreas> areasList = null;

    [System.Serializable]
    public class VerticalListableAreas
    {
        [HideInInspector] public string name = "Vertical List";

        [SerializeField] private List<SelectableArea> items = new List<SelectableArea>();

        public SelectableArea this[int _index] => items[_index];
    }

}
