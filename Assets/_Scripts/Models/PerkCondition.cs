using System;
using UnityEngine;

public abstract class PerkCondition
{
    //When a perk is added, they subscribe to a GA
    //When a perk is removed, we have to remove the subscription

    [SerializeField] protected ReactionTiming reactionTiming;
    public abstract void SubscribeCondition(Action<GameAction> reaction);
    public abstract void UnsubscribeCondition(Action<GameAction> reaction);
    public abstract bool SubConditionIsMet(GameAction gameAction);
}
