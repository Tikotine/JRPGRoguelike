using UnityEngine;
using System.Collections.Generic;

public class HeroTM : TargetMode
{
    //WILL NEED TO BE ADJUSTED TO ACCOUNT FOR MULTIPLE HEROS
    public override List<CombatantView> GetTargets()
    {
        List<CombatantView> targets = new()
        {
        HeroSystem.Instance.HeroView
        };

        return targets;
    }
}
