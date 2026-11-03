using System.Collections.Generic;
using UnityEngine;

public class RandomEnemyTM : TargetMode
{
    //TAKE A RANDOM ENEMY AND RETURN IT IN A LIST
    //Always return a list so we get a uniform output of a list
    public override List<CombatantView> GetTargets()
    {
        CombatantView target = EnemySystem.Instance.Enemies[Random.Range(0, EnemySystem.Instance.Enemies.Count)];
        return new() { target };
    }
}
