using DG.Tweening;
using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [Header("References")]
    [SerializeField] private CardView cardViewPrefab;

    [Header("Tweens")]
    [SerializeField] private Vector3 overshootScale = new Vector3(1.1f, 1.1f, 1.1f);

    //Method to create the card views
    public CardView CreateCardView(Vector3 position, Quaternion rotation)
    { 
        CardView cardView = Instantiate(cardViewPrefab, position, rotation);

        cardView.transform.localScale = Vector3.zero;
        cardView.transform.DOScale(overshootScale, 0.2f);
        cardView.transform.DOScale(Vector3.one, 0.3f);

        return cardView;
    }
}
