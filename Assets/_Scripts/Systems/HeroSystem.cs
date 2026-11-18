using UnityEngine;

public class HeroSystem : Singleton<HeroSystem>
{
    [Header("Values")]
    [SerializeField] private int freshHandCardCount = 5;

    [field: SerializeField] public HeroView HeroView { get; private set; }

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public void Setup(HeroData heroData)
    { 
        HeroView.Setup(heroData);
    }

    //Reactions
    //Call them as a reaction to the EnemyTurnGA
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        Debug.Log("Enemy Turn Pre");
        DiscardAllCardsGA discardAllCardsGA = new();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }

    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        int burnStacks = HeroView.GetStatusEffectStacks(StatusEffectType.BURN);
        if (burnStacks > 0)
        { 
            ApplyBurnGA applyBurnGA = new(burnStacks, HeroView);
            ActionSystem.Instance.AddReaction(applyBurnGA);
        }

        Debug.Log("Enemy Turn Post");
        DrawCardsGA drawCardsGA = new(freshHandCardCount);
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }
}
