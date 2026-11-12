using UnityEngine;

public interface IHaveCaster
{
    //IF ALL ACTIONS WILL HAVE A CASTER, THEN IMPLEMENT IN GAMEACTION CLASS
    CombatantView Caster { get; }
}
