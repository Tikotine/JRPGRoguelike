using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class HandView : MonoBehaviour
{
    [Header("Tweens")]
    [SerializeField] private float addCardDuration = 0.15f;
    [SerializeField] private float removeCardDuration = 0.15f;


    [SerializeField] private SplineContainer splineContainer;
    private readonly List<CardView> cards = new();

    //Take a card view and add it to the card list
    public IEnumerator AddCard(CardView cardView)
    { 
        cards.Add(cardView);
        yield return UpdateCardPositions(addCardDuration);

        yield break;
    }

    //Takes the card as parameter then removes it from hand after checking where it is
    public CardView RemoveCard(Card card)
    {
        CardView cardView = GetCardView(card);
        if (cardView == null)
        {
            return null;
        }
        cards.Remove(cardView);
        StartCoroutine(UpdateCardPositions(removeCardDuration));  //Timing here
        return cardView;
    }

    //Looks for the card in all cardviews
    private CardView GetCardView(Card card)
    {
        return cards.Where(cardView => cardView.Card == card).FirstOrDefault();
    }

    //Handle the position of the cards in the players hand
    private IEnumerator UpdateCardPositions(float duration)
    { 
        //If no cards, return
        if (cards.Count == 0) 
        { 
            yield break; 
        }

        //Calculate spacing between cards based on max hand size (10)
        float cardSpacing = 1f / 10f;
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;

        //Calculate the position and rotation for each card on the spline
        for (int i = 0; i < cards.Count; i++) 
        {
            float p = firstCardPosition + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);
            cards[i].transform.DOMove(splinePosition + transform.position + 0.01f * i * Vector3.back, duration).SetEase(Ease.OutQuart, 0.2f);
            cards[i].transform.DORotate(rotation.eulerAngles, duration).SetEase(Ease.OutQuart);
        }

        yield return new WaitForSeconds(duration);

        yield break;
    }
}
