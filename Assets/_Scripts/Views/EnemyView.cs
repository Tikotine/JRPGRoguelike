using TMPro;
using UnityEngine;

public class EnemyView : CombatantView
{
    [SerializeField] private TMP_Text attackText;
    public int AttackPower { get; set; }
    public EnemyData Data { get; private set; }
    public void Setup(EnemyData enemyData)
    {
        Data = enemyData;
        AttackPower = enemyData.AttackPower;
        UpdateAttackText();
        SetupBase(enemyData.Health, enemyData.Image, enemyData.Speed, enemyData.ActionBarImage);
    }

    private void UpdateAttackText()
    { 
        attackText.text = "ATK: " + AttackPower;
    }
}
