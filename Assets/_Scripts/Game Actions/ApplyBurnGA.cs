using UnityEngine;

public class ApplyBurnGA : GameAction
{
    //THIS WILL NOT APPLY THE STATUS EFFECT BUT ONLY EXECUTE THE BURN EFFECT
    //APPLYING THE STATUS EFFECT WILL BE DONE THROUGH THE ADD STATUS EFFECT GA
    public int BurnDamage { get; private set; }
    public CombatantView Target { get; private set; }
    public ApplyBurnGA(int burnDamage, CombatantView target)
    {
        BurnDamage = burnDamage;
        Target = target;
    }
}
