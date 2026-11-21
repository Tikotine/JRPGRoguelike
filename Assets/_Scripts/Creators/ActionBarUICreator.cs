using UnityEngine;
using UnityEngine.UI;

public class ActionBarUICreator : Singleton<ActionBarUICreator>
{
    [SerializeField] private Slider actionBarUIPrefab;
    [SerializeField] private RectTransform actionBarsUI;

    public bool HasReferences => actionBarUIPrefab != null && actionBarsUI != null;
    public float MinValue => actionBarUIPrefab.minValue;
    public float MaxValue => actionBarUIPrefab.maxValue;

    public Slider CreateActionBarUI(Sprite actionBarImage)
    {
        Slider bar = Instantiate(actionBarUIPrefab, actionBarsUI);
        bar.interactable = false;

        Transform characterImageTransform = bar.transform.Find("Handle Slide Area/Handle/CharacterImage");
        if (characterImageTransform != null && characterImageTransform.TryGetComponent(out Image characterImage))
        {
            characterImage.sprite = actionBarImage;
        }

        return bar;
    }
}
