using UnityEngine;

//Played when a hero becomes the active unit. Other systems react to this to draw, refill mana, and tick burn.
public class HeroTurnStartGA : GameAction
{
    public HeroView Hero { get; private set; }

    public HeroTurnStartGA(HeroView hero)
    {
        Hero = hero;
    }
}
