using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class TestSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<CardData> deckData;
    [SerializeField] private Button endTurnButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CardSystem.Instance.Setup(deckData);
    }
}
