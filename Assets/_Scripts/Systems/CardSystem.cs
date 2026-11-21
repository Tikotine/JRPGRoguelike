using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardSystem : Singleton<CardSystem>
{
    [Header("Tweens")]
    [SerializeField] private float discardFinalScaleDuration = 0.15f;
    [SerializeField] private float discardFinalMoveDuration = 0.15f;

    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;
    private readonly Dictionary<HeroView, HeroDeck> heroDecks = new();
    private readonly HeroDeck sharedDeck = new();
    private HeroDeck activeDeck;
    private HeroDeck ActiveDeck => activeDeck != null ? activeDeck : sharedDeck;
    private readonly List<Card> hand = new();


    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
    }

    //Publics
    public void Setup(List<CardData> deckData)
    {
        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            sharedDeck.DrawPile.Add(card);
        }

        if (activeDeck == null)
        {
            activeDeck = sharedDeck;
        }
    }

    //Build this hero's own draw pile from their HeroData deck.
    public void SetupHero(HeroView hero)
    {
        if (hero == null)
        {
            return;
        }

        HeroDeck deck = new();
        if (hero.Data != null && hero.Data.Deck != null)
        {
            foreach (CardData cardData in hero.Data.Deck)
            {
                deck.DrawPile.Add(new Card(cardData));
            }
        }

        heroDecks.Add(hero, deck);
    }

    //Draws and discards on the hero whose turn it is.
    public void SetActiveHero(HeroView hero)
    {
        if (hero != null && heroDecks.TryGetValue(hero, out HeroDeck deck))
        {
            activeDeck = deck;
        }
    }

    //Performers (Any logic behind the GA)
    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {
        //Calculate how many cards can be drawn from the deck efore the deck needs to e reshuffled
        int actualAmount = Mathf.Min(drawCardsGA.Amount, ActiveDeck.DrawPile.Count);
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
        //Check if theres a manual target effect
        //Other effects will always be called regardless of the manual target effect
        if (playCardGA.Card.ManualTargetEffect != null)
        {
            //Pass the effect and target
            PerformEffectGA performEffectGA = new(playCardGA.Card.ManualTargetEffect, new() { playCardGA.ManualTarget });
            ActionSystem.Instance.AddReaction(performEffectGA);
        }

        //Iterate through the list of effects and create a PerformEffectGA for each effect
        foreach (var effectWrapper in playCardGA.Card.OtherEffects)
        {
            List<CombatantView> targets = effectWrapper.TargetMode.GetTargets();
            PerformEffectGA performEffectGA = new(effectWrapper.Effect, targets);
            //DO NOT USE THE PERFORM METHOD BECAUSE WE ARE IN ANOTHER PERFORMER
            //If we want to perform a GameAction inside another GameAction's performer, we must always use the AddReaction method
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
    }

    //Helpers
    private IEnumerator DrawCard()
    {
        Card card = ActiveDeck.DrawPile.Draw();
        hand.Add(card);
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
        yield return handView.AddCard(cardView);
    }

    //Adds all cards from discard pile to draw pile and clears list
    private void RefillDeck()
    {
        ActiveDeck.DrawPile.AddRange(ActiveDeck.DiscardPile);
        ActiveDeck.DiscardPile.Clear();
    }

    private IEnumerator DiscardCard(CardView cardView)
    {
        ActiveDeck.DiscardPile.Add(cardView.Card);
        cardView.transform.DOScale(Vector3.zero, discardFinalScaleDuration).SetEase(Ease.OutQuart);
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, discardFinalMoveDuration).SetEase(Ease.OutQuart);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
    }

    private sealed class HeroDeck
    {
        public readonly List<Card> DrawPile = new();
        public readonly List<Card> DiscardPile = new();
    }
}
