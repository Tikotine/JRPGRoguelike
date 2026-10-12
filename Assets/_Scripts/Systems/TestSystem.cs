using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TestSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<CardData> deckData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CardSystem.Instance.Setup(deckData);
    }
}
