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
}
