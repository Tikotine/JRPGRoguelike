using UnityEngine;

public class AttackHeroGA : GameAction, IHaveCaster
{
    public EnemyView Attacker { get; private set; }
    public HeroView Target { get; private set; }
    public CombatantView Caster { get; private set; }
    public AttackHeroGA(EnemyView attacker, HeroView target)
    {
        Attacker = attacker;
        Target = target;
        Caster = Attacker;
    }
}
