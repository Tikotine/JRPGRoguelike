using UnityEngine;

public class CardViewHoverSystem : Singleton<CardViewHoverSystem>
{
    //Specifically for the hover only
    [SerializeField] private CardView cardViewHover;

    protected override void Awake()
    {
        base.Awake();

        //The preview sits in front of the hand card. Leave its collider off so the pointer ray stays on the card underneath.
        if (cardViewHover.TryGetComponent(out Collider collider))
        {
            collider.enabled = false;
        }
    }

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
