using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text mana;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapperGO;

    public Card Card { get; private set; }

    //Constructor for the CardView Based on the "Card"'s information
    public void Setup(Card card)
    {
        Card = card;
        title.text = card.Title;
        description.text = card.Desciption;
        mana.text = card.Mana.ToString();
        imageSR.sprite = card.Image;
    }
}
