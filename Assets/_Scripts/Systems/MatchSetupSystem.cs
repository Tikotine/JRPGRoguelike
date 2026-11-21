using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : MonoBehaviour
{
    [SerializeField] private List<HeroData> heroDatas; //Heroes fighting this match
    [SerializeField] private PerkData perkData;
    [SerializeField] private List<EnemyData> enemyDatas;

    private void Start()
    {
        HeroSystem.Instance.Setup(heroDatas);
        EnemySystem.Instance.Setup(enemyDatas);
        if (heroDatas != null && heroDatas.Count > 0)
        {
            CardSystem.Instance.Setup(heroDatas[0].Deck);
        }
        PerkSystem.Instance.AddPerk(new Perk(perkData));
        ActionBarSystem.Instance.BeginBattle();
    }
}
