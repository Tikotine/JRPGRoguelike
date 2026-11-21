using TMPro;
using UnityEngine;

public class HeroView : CombatantView
{
    public HeroData Data { get; private set; }

    public void Setup(HeroData heroData)
    {
        Data = heroData;
        SetupBase(heroData.Health, heroData.Image, heroData.Speed, heroData.ActionBarImage);
    }

}
