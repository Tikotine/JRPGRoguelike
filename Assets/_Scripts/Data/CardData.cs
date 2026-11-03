using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/Card")]

public class CardData : ScriptableObject
{
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public int Mana { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }

    //More data like effects etc go here
    [field: SerializeReference, SR] public Effect ManualTargetEffect { get; private set; } = null;  //Every card can have one manual target effect with a target arrow

    //Wrapper for all other effects
    [field: SerializeField] public List<AutoTargetEffect> OtherEffects { get; private set; }    //The card can also have multiple other effects where the target is selected randomly
}
