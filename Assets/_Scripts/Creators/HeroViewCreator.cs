using UnityEngine;

public class HeroViewCreator : Singleton<HeroViewCreator>
{
    [SerializeField] private HeroView heroViewPrefab;

    public HeroView CreateHeroView(HeroData heroData, Vector3 position, Quaternion rotation)
    {
        HeroView heroView = Instantiate(heroViewPrefab, position, rotation);
        heroView.Setup(heroData);
        return heroView;
    }
}
