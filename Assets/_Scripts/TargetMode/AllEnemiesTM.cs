using UnityEngine;
using System.Collections.Generic;

public class AllEnemiesTM : TargetMode
{
    //RETURN A LIST OF ALL ENEMIES
    public override List<CombatantView> GetTargets()
    {
        return new(EnemySystem.Instance.Enemies);
    }
}
