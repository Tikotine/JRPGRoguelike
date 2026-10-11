using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

    private void OnMouseEnter()
    {
        wrapperGO.SetActive(false);
        Vector3 pos = new(transform.position.x, transform.position.y, -0.5f);   //Adjust where the hover will appear here
        CardViewHoverSystem.Instance.Show(Card, pos);
    }
    private void OnMouseExit() 
    {
        CardViewHoverSystem.Instance.Hide();
        wrapperGO.SetActive(true);
    }
}
