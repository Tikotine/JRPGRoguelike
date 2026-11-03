using UnityEngine;
using System.Collections.Generic;

public class DrawCardsEffect : Effect
{
    [SerializeField] private int drawAmount;
    public override GameAction GetGameAction(List<CombatantView> targets)   //Have the targets parameter but we wont use it
    {
        DrawCardsGA drawCardsGA = new(drawAmount);
        return drawCardsGA;
    }
}
