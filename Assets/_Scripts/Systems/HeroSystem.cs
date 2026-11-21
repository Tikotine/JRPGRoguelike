using System.Collections.Generic;
using UnityEngine;

public class HeroSystem : Singleton<HeroSystem>
{
    [Header("Values")]
    [SerializeField] private int freshHandCardCount = 5;

    [field: SerializeField] public HeroView HeroView { get; private set; }
    [SerializeField] private List<HeroView> additionalHeroViews = new();

    //Every hero that was set up for this match. The inspector hero is first, then any extra views.
    private readonly List<HeroView> heroes = new();
    public IReadOnlyList<HeroView> Heroes => heroes;

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<HeroTurnEndGA>(HeroTurnEndReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<HeroTurnStartGA>(HeroTurnStartReaction, ReactionTiming.PRE);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<HeroTurnEndGA>(HeroTurnEndReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<HeroTurnStartGA>(HeroTurnStartReaction, ReactionTiming.PRE);
    }

    public void Setup(List<HeroData> heroDatas)
    {
        heroes.Clear();
        if (heroDatas == null)
        {
            Debug.Log("No Hero Datas");
            return;
        }

        //Pair each hero data with a view. The inspector hero is first, then any extra views dragged in for later heroes.
        List<HeroView> views = new();
        if (HeroView != null)
        {
            views.Add(HeroView);
        }

        if (additionalHeroViews != null)
        {
            foreach (HeroView extraHero in additionalHeroViews)
            {
                if (extraHero != null && !views.Contains(extraHero))
                {
                    views.Add(extraHero);
                }
            }
        }

        int count = Mathf.Min(heroDatas.Count, views.Count);
        for (int i = 0; i < count; i++)
        {
            if (heroDatas[i] == null)
            {
                continue;
            }

            views[i].Setup(heroDatas[i]);
            heroes.Add(views[i]);
        }
    }

    //Reactions
    //Discard when that hero's turn ends. Draw and burn when their next turn starts.
    private void HeroTurnEndReaction(HeroTurnEndGA heroTurnEndGA)
    {
        Debug.Log("Hero Turn End");
        DiscardAllCardsGA discardAllCardsGA = new();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }

    private void HeroTurnStartReaction(HeroTurnStartGA heroTurnStartGA)
    {
        HeroView hero = heroTurnStartGA.Hero != null ? heroTurnStartGA.Hero : HeroView;
        int burnStacks = hero.GetStatusEffectStacks(StatusEffectType.BURN);
        if (burnStacks > 0)
        { 
            ApplyBurnGA applyBurnGA = new(burnStacks, hero);
            ActionSystem.Instance.AddReaction(applyBurnGA);
        }

        Debug.Log("Hero Turn Start");
        DrawCardsGA drawCardsGA = new(freshHandCardCount);
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }
}
