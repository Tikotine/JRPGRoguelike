using TMPro;
using UnityEngine;

public class EnemyView : CombatantView
{
    [SerializeField] private TMP_Text attackText;
    public int AttackPower { get; set; }
    public void Setup()
    { 
        AttackPower = 10;   //Dummy data, to be replaced
        UpdateAttackText();
        SetupBase(CurrentHealth, null);
    }

    private void UpdateAttackText()
    { 
        attackText.text = "ATK: " + AttackPower;
    }
}
