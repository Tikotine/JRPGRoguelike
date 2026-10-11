using UnityEngine;

public class CardViewHoverSystem : Singleton<CardViewHoverSystem>
{
    //Specifically for the hover only
    [SerializeField] private CardView cardViewHover;

    //Enables the card view and sets it up
    public void Show(Card card, Vector3 position)
    { 
        cardViewHover.gameObject.SetActive(true);
        cardViewHover.Setup(card);
        cardViewHover.transform.position = position;
    }

    public void Hide()
    { 
        cardViewHover.gameObject.SetActive(false);
    }
}
