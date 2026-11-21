using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    public List<EnemyView> Enemies => enemyBoardView.EnemyViews;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<EnemyActGA>(EnemyActPerformer);
        ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer);
        ActionSystem.AttachPerformer<KillEnemyGA>(KillEnemyPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<EnemyActGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();
    }

    public void Setup(List<EnemyData> enemyDatas)
    {
        foreach (var enemyData in enemyDatas)
        {
            enemyBoardView.AddEnemy(enemyData);
        }
    }

    //Performers
    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemuTurnGA)
    {
        foreach (var enemy in enemyBoardView.EnemyViews)    //Create a GA for each enemy we have
        {
            int burnStacks = enemy.GetStatusEffectStacks(StatusEffectType.BURN);
            if (burnStacks > 0)
            { 
                ApplyBurnGA applyBurnGA = new(burnStacks, enemy);
                ActionSystem.Instance.AddReaction(applyBurnGA);
            }

            HeroView target = GetRandomLivingHero();
            if (target != null)
            {
                AttackHeroGA attackHeroGA = new(enemy, target);
                ActionSystem.Instance.AddReaction(attackHeroGA);
            }
        }
        yield return null;
    }

    //The action bar asks one enemy to act. Burn ticks first, then they attack.
    private IEnumerator EnemyActPerformer(EnemyActGA enemyActGA)
    {
        EnemyView enemy = enemyActGA.Enemy;
        int burnStacks = enemy.GetStatusEffectStacks(StatusEffectType.BURN);
        if (burnStacks > 0)
        {
            ApplyBurnGA applyBurnGA = new(burnStacks, enemy);
            ActionSystem.Instance.AddReaction(applyBurnGA);
        }

        HeroView target = GetRandomLivingHero();
        if (target != null)
        {
            AttackHeroGA attackHeroGA = new(enemy, target);
            ActionSystem.Instance.AddReaction(attackHeroGA);
        }
        yield return null;
    }

    private IEnumerator AttackHeroPerformer(AttackHeroGA attackHeroGA)
    {
        EnemyView attacker = attackHeroGA.Attacker;

        //Animate the attack
        Tween tween = attacker.transform.DOMoveX(attacker.transform.position.x - 1f, 0.15f);    //Move forward
        yield return tween.WaitForCompletion(); //Wait
        attacker.transform.DOMoveX(attacker.transform.position.x + 1f, 0.25f); //Move back

        //Do Damage
        if (attackHeroGA.Target != null && attackHeroGA.Target.CurrentHealth > 0)
        {
            DealDamageGA dealDamageGA = new(attacker.AttackPower, new() { attackHeroGA.Target }, attackHeroGA.Caster);
            ActionSystem.Instance.AddReaction(dealDamageGA);
        }
    }

    //Pick one hero who is still alive. Returns null when the party is already defeated.
    private HeroView GetRandomLivingHero()
    {
        List<HeroView> livingHeroes = new();
        foreach (HeroView hero in HeroSystem.Instance.Heroes)
        {
            if (hero != null && hero.CurrentHealth > 0)
            {
                livingHeroes.Add(hero);
            }
        }

        if (livingHeroes.Count == 0)
        {
            return null;
        }

        return livingHeroes[Random.Range(0, livingHeroes.Count)];
    }

    private IEnumerator KillEnemyPerformer(KillEnemyGA killEnemyGA)
    {
        yield return enemyBoardView.RemoveEnemy(killEnemyGA.EnemyView);
    }
}
