using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class HandView : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    private readonly List<CardView> cards = new();

    //Take a card view and add it to the card list
    public IEnumerator AddCard(CardView cardView)
    { 
        cards.Add(cardView);
        yield return UpdateCardPositions(0.15f);

        yield break;
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
            cards[i].transform.DORotate(rotation.eulerAngles, duration).SetEase(Ease.OutQuart, 0.2f);
        }

        yield return new WaitForSeconds(duration);

        yield break;
    }
}
