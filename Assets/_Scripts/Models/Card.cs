using System.Collections.Generic;
using UnityEngine;

public class Card
{
    //Values that will not change
    public string Title => data.name;
    public string Desciption => data.Description;
    public Sprite Image => data.Image;
    public Effect ManualTargetEffect => data.ManualTargetEffect;
    public List<AutoTargetEffect> OtherEffects => data.OtherEffects;

    //Values that can change (Change it only for this card instance)
    public int Mana { get; private set; }


    private readonly CardData data;
    public Card(CardData cardData)
    { 
        data = cardData;
        Mana = cardData.Mana;
    }
}
