using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSystem : Singleton<HeroSystem>
{
    [Header("Values")]
    [SerializeField] private int freshHandCardCount = 5;
    [SerializeField] private HeroBoardView heroBoardView;

    //The hero whose turn is currently being played. Falls back to the first living hero.
    private HeroView activeHero;
    public HeroView HeroView
    {
        get
        {
            if (activeHero != null && activeHero.CurrentHealth > 0)
            {
                return activeHero;
            }

            foreach (HeroView hero in Heroes)
            {
                if (hero != null && hero.CurrentHealth > 0)
                {
                    return hero;
                }
            }

            return null;
        }
    }

    public IReadOnlyList<HeroView> Heroes => heroBoardView.HeroViews;

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<HeroTurnEndGA>(HeroTurnEndReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<HeroTurnStartGA>(HeroTurnStartReaction, ReactionTiming.PRE);
        ActionSystem.AttachPerformer<KillHeroGA>(KillHeroPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<HeroTurnEndGA>(HeroTurnEndReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<HeroTurnStartGA>(HeroTurnStartReaction, ReactionTiming.PRE);
        ActionSystem.DetachPerformer<KillHeroGA>();
    }

    public void Setup(List<HeroData> heroDatas)
    {
        if (heroDatas == null)
        {
            Debug.Log("No Hero Datas");
            return;
        }

        foreach (HeroData heroData in heroDatas)
        {
            if (heroData == null)
            {
                continue;
            }

            heroBoardView.AddHero(heroData);
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
        activeHero = hero;
        CardSystem.Instance.SetActiveHero(hero);

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

    private IEnumerator KillHeroPerformer(KillHeroGA killHeroGA)
    {
        if (activeHero == killHeroGA.HeroView)
        {
            activeHero = null;
        }

        yield return heroBoardView.RemoveHero(killHeroGA.HeroView);
    }
}
