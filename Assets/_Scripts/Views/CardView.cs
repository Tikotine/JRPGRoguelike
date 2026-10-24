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
    [SerializeField] private LayerMask dropLayer;

    [Header("Values")]
    [SerializeField] private float cardPlayDistance = 10f;

    //Store pos and rotation in the moment we start to drag
    private Vector3 dragStartPosition;
    private Quaternion dragStartRotation;
    private bool isHovering;
    private bool isDragging;

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

    private void Update()
    {
        bool pointerOverThis = PlayerInput.Instance.TryGetColliderUnderPointer(out Collider hit)
            && hit.gameObject == gameObject;

        if (pointerOverThis && !isHovering)
        {
            isHovering = true;
            OnPointerEnter();
        }
        else if (!pointerOverThis && isHovering)
        {
            isHovering = false;
            OnPointerExit();
        }

        if (!isDragging && pointerOverThis && PlayerInput.Instance.WasClickPressedThisFrame())
        {
            OnPointerDown();
        }

        if (isDragging && PlayerInput.Instance.IsClickHeld())
        {
            OnPointerDrag();
        }

        if (isDragging && PlayerInput.Instance.WasClickReleasedThisFrame())
        {
            isDragging = false;
            OnPointerUp();
        }
    }

    private void OnPointerEnter()
    {
        if (!Interactions.Instance.PlayerCanHover())
        {
            return;
        }

        wrapperGO.SetActive(false);
        Vector3 pos = new(transform.position.x, transform.position.y, -0.5f);   //Adjust where the hover will appear here
        CardViewHoverSystem.Instance.Show(Card, pos);
    }
    private void OnPointerExit() 
    {
        if (!Interactions.Instance.PlayerCanHover())
        {
            return;
        }

        CardViewHoverSystem.Instance.Hide();
        wrapperGO.SetActive(true);
    }

    private void OnPointerDown()
    {
        if (!Interactions.Instance.PlayerCanInteract())
        {
            return;
        }

        isDragging = true;
        Interactions.Instance.PlayerIsDragging = true;
        wrapperGO.SetActive(true);
        CardViewHoverSystem.Instance.Hide();
        
        //Store initial values
        dragStartPosition = transform.position;
        dragStartRotation = transform.rotation;

        //Set values
        transform.rotation = Quaternion.Euler(20, 0, 0);
        transform.position = MouseUtil.GetMousePositionInWorldSpace();  //Tutorial put -1 to be closet to the orthographic camera
    }

    private void OnPointerDrag()
    {
        if (!Interactions.Instance.PlayerCanInteract())
        {
            return;
        }

        transform.position = MouseUtil.GetMousePositionInWorldSpace();
    }

    private void OnPointerUp()
    {
        if (!Interactions.Instance.PlayerCanInteract())
        {
            return;
        }

        if (ManaSystem.Instance.HasEnoughMana(Card.Mana) && Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, cardPlayDistance, dropLayer))
        {
            //Play Card
            Debug.Log("Play Card");
            PlayCardGA playCardGA = new(Card);
            ActionSystem.Instance.Perform(playCardGA);
        }

        else
        {
            transform.position = dragStartPosition;
            transform.rotation = dragStartRotation;
        }
        Interactions.Instance.PlayerIsDragging = false;
    }
}
