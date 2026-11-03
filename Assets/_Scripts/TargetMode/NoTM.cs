using System.Collections.Generic;
using UnityEngine;

public class NoTM : TargetMode
{
    //FOR CARDS WITH NO TARGET MODE LIKE CARD DRAW
    public override List<CombatantView> GetTargets()
    {
        return null;
    }
}
