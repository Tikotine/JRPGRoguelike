using UnityEngine;

public class KillHeroGA : GameAction
{
    public HeroView HeroView { get; private set; }
    public KillHeroGA(HeroView heroView)
    {
        HeroView = heroView;
    }
}
