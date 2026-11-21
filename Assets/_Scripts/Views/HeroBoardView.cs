using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBoardView : MonoBehaviour
{
    [SerializeField] private List<Transform> slots;
    public List<HeroView> HeroViews { get; private set; } = new();

    public void AddHero(HeroData heroData)   //Change this to an Ienumerator if i want an animation on spawn
    {
        Transform slot = slots[HeroViews.Count];
        HeroView heroView = HeroViewCreator.Instance.CreateHeroView(heroData, slot.position, slot.rotation);
        heroView.transform.parent = slot;
        HeroViews.Add(heroView);
    }

    public IEnumerator RemoveHero(HeroView heroView)
    {
        HeroViews.Remove(heroView);

        Tween tween = heroView.transform.DOScale(Vector3.zero, 0.25f); //TEMP DEATH ANIM
        yield return tween.WaitForCompletion();

        Destroy(heroView.gameObject);
    }
}
