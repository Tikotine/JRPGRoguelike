using UnityEngine;
using SerializeReferenceEditor;

[CreateAssetMenu(menuName = "Data/Perk")]
public class PerkData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeReference, SR] public PerkCondition PerkCondition { get; private set; }
    [field: SerializeReference, SR] public AutoTargetEffect AutoTargetEffect { get; private set; }
    [field: SerializeField] public bool UseAutoTarget { get; private set; } = true; //Use the AutoTarget mode from the effect
    [field: SerializeField] public bool UseActionCasterAsTarget { get; private set; } = false;  //Allows us to use the caster as a target
}
