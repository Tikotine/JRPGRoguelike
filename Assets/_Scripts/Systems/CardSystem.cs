using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardSystem : Singleton<CardSystem>
{
    [Header("Values")]
    [SerializeField] private int  freshHandCardCount = 5;

    [Header("Tweens")]
    [SerializeField] private float discardFinalScaleDuration = 0.15f;
    [SerializeField] private float discardFinalMoveDuration = 0.15f;

    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;
    private readonly List<Card> drawPile = new();
    private readonly List<Card> discardPile = new();
    private readonly List<Card> hand = new();


    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    //Publics
    public void Setup(List<CardData> deckData)
    {
        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
    }

    //Performers (Any logic behind the GA)
    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {
        //Calculate how many cards can be drawn from the deck efore the deck needs to e reshuffled
        int actualAmount = Mathf.Min(drawCardsGA.Amount, drawPile.Count);
        int notDrawnAmount = drawCardsGA.Amount - actualAmount; //To be drawn after the reshuffle
        for (int i = 0; i < actualAmount; i++)
        {
            yield return DrawCard();
        }

        if (notDrawnAmount > 0) //Shuffle then draw if needed
        {
            RefillDeck();

            for (int i = 0; i < notDrawnAmount; i++)
            {
                yield return DrawCard();
            }    
        }
    }

    //Go through each card in hand and add it to the discard pile. Remove it from hand.
    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {
        foreach (var card in hand)
        {
            discardPile.Add(card);
            CardView cardView = handView.RemoveCard(card);
            yield return DiscardCard(cardView);
        }

        hand.Clear();
    }

    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        //Remove the played card from the hand and the card view from the hand view
        hand.Remove(playCardGA.Card);
        CardView cardView = handView.RemoveCard(playCardGA.Card);
        yield return DiscardCard(cardView);

        //Spend Mana
        SpendManaGA spendManaGA = new(playCardGA.Card.Mana);
        ActionSystem.Instance.AddReaction(spendManaGA);

        //Perform Effects
        //Iterate through the list of effects and create a PerformEffectGA for each effect
        foreach (var effect in playCardGA.Card.Effects)
        {
            PerformEffectGA performEffectGA = new(effect);
            //DO NOT USE THE PERFORM METHOD BECAUSE WE ARE IN ANOTHER PERFORMER
            //If we want to perform a GameAction inside another GameAction's performer, we must always use the AddReaction method
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
    }

    //Reactions
    //Call them as a reaction to the EnemyTurnGA
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        Debug.Log("Enemy Turn Pre");
        DiscardAllCardsGA discardAllCardsGA = new();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }

    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        Debug.Log("Enemy Turn Post");
        DrawCardsGA drawCardsGA = new(freshHandCardCount);
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }

    //Helpers
    private IEnumerator DrawCard()
    {
        Card card = drawPile.Draw();
        hand.Add(card);
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
        yield return handView.AddCard(cardView);
    }

    //Adds all cards from discard pile to draw pile and clears list
    private void RefillDeck()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
    }

    private IEnumerator DiscardCard(CardView cardView)
    {
        cardView.transform.DOScale(Vector3.zero, discardFinalScaleDuration).SetEase(Ease.OutQuart);
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, discardFinalMoveDuration).SetEase(Ease.OutQuart);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
    }
}
