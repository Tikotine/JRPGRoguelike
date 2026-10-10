using UnityEngine;
using UnityEngine.InputSystem;

public class TestSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandView handView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        { 
        CardView cardView = CardViewCreator.Instance.CreateCardView(transform.position, Quaternion.identity);
        StartCoroutine(handView.AddCard(cardView));        
        }
    }
}
