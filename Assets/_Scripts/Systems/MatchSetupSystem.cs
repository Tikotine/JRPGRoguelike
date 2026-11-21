using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : Singleton<MatchSetupSystem>
{
    [SerializeField] private List<HeroData> heroDatas; //Heroes fighting this match
    [SerializeField] private PerkData perkData;
    [SerializeField] private List<EnemyData> enemyDatas;

    public IReadOnlyList<HeroData> HeroDatas => heroDatas;
    public IReadOnlyList<EnemyData> EnemyDatas => enemyDatas;

    private void Start()
    {
        HeroSystem.Instance.Setup(heroDatas);
        EnemySystem.Instance.Setup(enemyDatas);
        foreach (HeroView hero in HeroSystem.Instance.Heroes)
        {
            CardSystem.Instance.SetupHero(hero);
        }
        PerkSystem.Instance.AddPerk(new Perk(perkData));
        MatchStateSystem.Instance.Setup();
        ActionBarSystem.Instance.BeginBattle();
    }
}
