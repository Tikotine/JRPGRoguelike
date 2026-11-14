using System.Collections.Generic;
using UnityEngine;

public class StatusEffectsUI : MonoBehaviour
{
    [SerializeField] private StatusEffectUI statusEffectUIPrefab;
    [SerializeField] private Sprite armorSprite, burnSprite;
    private Dictionary<StatusEffectType, StatusEffectUI> statusEffectUIs = new();

    //One method that handles the updated stack count for a type and will handle the display logic
    public void UpdateStatusEffectUI(StatusEffectType statusEffectType, int stackCount)
    {
        //If stack count of 0, remove the UI if there is one
        if (stackCount == 0)
        {
            if (statusEffectUIs.ContainsKey(statusEffectType))
            { 
                StatusEffectUI statusEffectUI = statusEffectUIs[statusEffectType];
                statusEffectUIs.Remove(statusEffectType);
                Destroy(statusEffectUI.gameObject);
            }
        }

        else 
        {
            //Check if no UI, if no UI, create and add it to the dictionary
            if (!statusEffectUIs.ContainsKey(statusEffectType))
            {
                StatusEffectUI statusEffectUI = Instantiate(statusEffectUIPrefab, transform);
                statusEffectUIs.Add(statusEffectType, statusEffectUI);
            }

            //Get the sprite, set it and update stack count
            Sprite sprite = GetSpriteByType(statusEffectType);
            statusEffectUIs[statusEffectType].Set(sprite, stackCount);
        }
    }

    //Helper Method
    //CHANGE ALL EFFECT SPRITES HERE
    private Sprite GetSpriteByType(StatusEffectType statusEffectType) 
    {
        return statusEffectType switch
        {
            StatusEffectType.ARMOR => armorSprite,
            StatusEffectType.BURN => burnSprite,
            _ => null,
        };
    }
}
