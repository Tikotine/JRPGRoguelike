using System.Collections.Generic;
using UnityEngine;

public abstract class GameAction
{
    //Action Queue Lists
    public List<GameAction> PreReactions { get; set; } = new();     //Defore Action
    public List<GameAction> PerformReactions { get; set; } = new();     //During Action
    public List<GameAction> PostReactions { get; set; } = new();    //Post Action
}
