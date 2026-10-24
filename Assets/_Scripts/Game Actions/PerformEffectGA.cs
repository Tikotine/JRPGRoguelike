using UnityEngine;

public class PerformEffectGA : GameAction
{
    public Effect Effect { get; set; }

    //Will only hold the effect we want to perform
    public PerformEffectGA(Effect effect)
    { 
        Effect = effect;
    }
}
