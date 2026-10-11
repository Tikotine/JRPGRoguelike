using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.VisualScripting;

public class ActionSystem : Singleton<ActionSystem>
{
    //https://www.youtube.com/watch?v=ls5zeiDCfvI 11:10

    private List<GameAction> reactions = null;  //To reference the lists inside GameActions. Reference the list of the current phase so the reactions can be added
    public bool IsPerforming { get; private set; } = false;     //For allowing interactions with cards when the system is/is not performing
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();    //Subscribers to the pre-phase of the game action (Before a game action)
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();   //Subscribers to the post-phase of the game action (After a game action)
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();  //Hold the logic for game actions. Game actions only contain data, no logic. For each type, attach a performer that is called when an action of that type is performed.
    public void Perform(GameAction action, System.Action OnPerformFinished = null)      //Give an action and an optional callback that is called when Perform is completed
    {
        if (IsPerforming)   //Only run when no other action is performing
        { 
            return;
        }

        IsPerforming = true;
        StartCoroutine(Flow(action, () =>
        {
            //On Flow completed
            IsPerforming = false;
            OnPerformFinished?.Invoke();
        }));
    }

    //Gives the ability to add reactions from other classes. Always adds to the CURRENT list
    public void AddReaction(GameAction gameAction)
    { 
        reactions?.Add(gameAction);
    }

    //THE ENTIRE FLOW OF THE TURN SYSTEM
    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    { 
        reactions = action.PreReactions;    //First, set reactions list to pre-reactions
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();

        reactions = action.PerformReactions;    //Swap list before performing performers
        yield return PerformPerformer(action);  //Perform performer first in case any performer adds a reaction
        yield return PerformReactions();

        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();

        OnFlowFinished?.Invoke();
    }

    #region PERFORMER MANAGEMENT
    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
    }

    //Tells all subscribers of this game action type that we are performing this action in X phase
    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            foreach (var sub in subs[type])
            {
                sub(action);
            }
        }
    }

    //Calls another Flow method for each reaction until there are no more reactions to resolve
    private IEnumerator PerformReactions()
    {
        foreach (var reaction in reactions)
        {
            yield return Flow(reaction);
        }
    }


    //Carries out the logic of the Game Action
    //Per Game Action Type, You can ONLY HAVE ONE PERFORMER
    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    { 
        Type type = typeof(T);  //Get the type
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);    //Convert it so that it can be added to the Dictionary
        if (performers.ContainsKey(type))
        {
            performers[type] = wrappedPerformer;
        }

        else 
        {
            performers.Add(type, wrappedPerformer);
        }
    }

    public static void DetachPerformer<T>() where T : GameAction
    { 
        Type type = typeof(T);
        if (performers.ContainsKey(type)) 
        {
            performers.Remove(type);
        }
    }

    //Sub and unsub reactions to a type of Game Action
    //Can have multiple reactions to one game action type
    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    { 
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        void wrappedReaction(GameAction action) => reaction((T)action);
        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(wrappedReaction);
        }

        else 
        {
            subs.Add(typeof(T), new());
            subs[typeof(T)].Add(wrappedReaction);
        }
    }

    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    { 
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }

    #endregion
}
