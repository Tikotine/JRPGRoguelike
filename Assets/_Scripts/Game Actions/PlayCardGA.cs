using UnityEngine;

public class PlayCardGA : GameAction
{
    public EnemyView ManualTarget { get; private set; }
    public Card Card { get; set; }

    //Constructor for actions without a manual target
    public PlayCardGA(Card card)
    {
        Card = card;
        ManualTarget = null;
    }

    //Constructor for actions with a manual target
    public PlayCardGA(Card card, EnemyView target)
    {
        Card = card;
        ManualTarget = target;
    }
}
