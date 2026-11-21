using UnityEngine;

//Played when the player presses End Turn. Other systems react to this to discard the hand.
public class HeroTurnEndGA : GameAction
{
    public HeroView Hero { get; private set; }

    public HeroTurnEndGA(HeroView hero)
    {
        Hero = hero;
    }
}
