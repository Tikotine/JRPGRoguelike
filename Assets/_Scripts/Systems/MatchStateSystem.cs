using System.Collections.Generic;
using UnityEngine;

//Tracks who is still alive in this match and ends the game when one side is gone.
public class MatchStateSystem : Singleton<MatchStateSystem>
{
    private readonly List<HeroData> heroes = new();
    private readonly List<EnemyData> enemies = new();
    private bool isMatchOver;

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<KillHeroGA>(OnHeroKilled, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<KillEnemyGA>(OnEnemyKilled, ReactionTiming.PRE);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<KillHeroGA>(OnHeroKilled, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<KillEnemyGA>(OnEnemyKilled, ReactionTiming.PRE);
    }

    //Copy the match roster. Removing a killed unit here does not change the setup lists.
    public void Setup()
    {
        heroes.Clear();
        enemies.Clear();
        isMatchOver = false;

        if (MatchSetupSystem.Instance.HeroDatas != null)
        {
            foreach (HeroData heroData in MatchSetupSystem.Instance.HeroDatas)
            {
                if (heroData != null)
                {
                    heroes.Add(heroData);
                }
            }
        }

        if (MatchSetupSystem.Instance.EnemyDatas != null)
        {
            foreach (EnemyData enemyData in MatchSetupSystem.Instance.EnemyDatas)
            {
                if (enemyData != null)
                {
                    enemies.Add(enemyData);
                }
            }
        }
    }

    private void OnHeroKilled(KillHeroGA killHeroGA)
    {
        if (killHeroGA.HeroView != null)
        {
            heroes.Remove(killHeroGA.HeroView.Data);
        }

        if (heroes.Count == 0)
        {
            PlayerLoseSequence();
        }
    }

    private void OnEnemyKilled(KillEnemyGA killEnemyGA)
    {
        if (killEnemyGA.EnemyView != null)
        {
            enemies.Remove(killEnemyGA.EnemyView.Data);
        }

        if (enemies.Count == 0)
        {
            PlayerWinSequence();
        }
    }

    public void PlayerWinSequence()
    {
        if (isMatchOver)
        {
            return;
        }

        isMatchOver = true;
        ActionSystem.Instance.Stop();
        Debug.Log("Win");
    }

    public void PlayerLoseSequence()
    {
        if (isMatchOver)
        {
            return;
        }

        isMatchOver = true;
        ActionSystem.Instance.Stop();
        Debug.Log("Lose");
    }
}
