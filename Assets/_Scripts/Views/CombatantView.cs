using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatantView : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private StatusEffectsUI statusEffectsUI;
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int Speed { get; private set; }
    public Sprite ActionBarImage { get; private set; }
    //Honkai Star Rail base action value. Higher speed means a smaller value and an earlier turn.
    public float BaseActionValue => 10000f / Mathf.Max(Speed, 1);
    private Dictionary<StatusEffectType, int> statusEffects = new();

    protected void SetupBase(int health, Sprite image, int speed, Sprite actionBarImage)
    {
        MaxHealth = CurrentHealth = health;
        Speed = speed;
        ActionBarImage = actionBarImage;
        spriteRenderer.sprite = image;
        UpdateHealthText();
    }

    private void UpdateHealthText()
    { 
        healthText.text = "HP: " + CurrentHealth;
    }

    public void Damage(int damageAmount)
    {
        //Armor Calculations
        int remainingDamage = damageAmount;
        int currentArmor = GetStatusEffectStacks(StatusEffectType.ARMOR);
        if (currentArmor > 0)
        {
            //Block Damage
            if (currentArmor >= damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, remainingDamage);
                remainingDamage = 0;
            }

            //Calculate remaining damage
            else if (currentArmor < damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, currentArmor);
                remainingDamage -= currentArmor;
            }
        }

        if (remainingDamage > 0)
        {
            //Health Calculations
            CurrentHealth -= remainingDamage;
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }
        }

        //Game Feel
        transform.DOShakePosition(0.2f, 0.5f);  //Shake sprite on damage taken
        UpdateHealthText();
    }

    //Add a stack amt to a status type and update the UI
    public void AddStatusEffect(StatusEffectType type, int stackCount) 
    {
        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type] += stackCount;
        }

        else
        { 
            statusEffects.Add(type, stackCount);
        }

        statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStacks(type));
    }

    //Remove a stack amount if we have the status effect type, and remove the type if there are no more stacks
    public void RemoveStatusEffect(StatusEffectType type, int stackCount)
    {
        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type] -= stackCount;

            if (statusEffects[type] <= 0)
            { 
                statusEffects.Remove(type);
            }
        }

        statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStacks(type));
    }

    public int GetStatusEffectStacks(StatusEffectType type)
    {
        if (statusEffects.ContainsKey(type))
        {
            return statusEffects[type];
        }

        else 
        {
            return 0;
        }
    }
}
