using UnityEngine;

public class EndTurnButtonUI : MonoBehaviour
{
    public void OnClick()
    {
        ActionBarSystem.Instance.EndHeroTurn();
    }
}
