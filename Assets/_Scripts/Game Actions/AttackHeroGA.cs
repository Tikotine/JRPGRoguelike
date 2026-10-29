using UnityEngine;

public class AttackHeroGA : GameAction
{
    public EnemyView Attacker { get; private set; }
    public AttackHeroGA(EnemyView attacker) //INCLUDE TARGET HERE FOR MULTIPLE HEROS
    {
        Attacker = attacker;
    }
}
