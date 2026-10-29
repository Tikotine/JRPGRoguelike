using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
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
            AttackHeroGA attackHeroGA = new(enemy);
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
        DealDamageGA dealDamageGA = new(attacker.AttackPower, new() { HeroSystem.Instance.HeroView });  //Attackers damage and list of targets, which is just the hero. Needs to be editted for multiple heros
        ActionSystem.Instance.AddReaction(dealDamageGA);
    }
}
