using UnityEngine;

public class AttackHeroGA : GameAction, IHaveCaster
{
    public EnemyView Attacker { get; private set; }
    public CombatantView Caster { get; private set; } 
    public AttackHeroGA(EnemyView attacker) //INCLUDE TARGET HERE FOR MULTIPLE HEROS
    {
        Attacker = attacker;
        Caster = Attacker;
    }
}
