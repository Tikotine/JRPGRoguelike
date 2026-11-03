using UnityEngine;
using System.Collections.Generic;

public class PerformEffectGA : GameAction
{
    public Effect Effect { get; set; }
    public List<CombatantView> Targets { get; set; }

    //Will only hold the effect we want to perform
    public PerformEffectGA(Effect effect, List<CombatantView> targets)
    { 
        Effect = effect;
        Targets = targets == null ? null : new(targets);    //From the NoTM set property to null if the list is null
    }
}
