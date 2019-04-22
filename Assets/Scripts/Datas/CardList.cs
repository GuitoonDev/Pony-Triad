using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardList", menuName = "ProtoTriad/CardList")]
public class CardList : ScriptableObject
{
    [SerializeField]
    private List<CardDatas> cardDatasList = null;
    public List<CardDatas> CardDatasList => cardDatasList;
}
