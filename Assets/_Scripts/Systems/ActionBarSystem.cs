using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Decides whose turn it is from speed and action value, and shows that order on the action bar.
public class ActionBarSystem : Singleton<ActionBarSystem>
{
    [Header("Layout")]
    [SerializeField] private int maxActionBarUIs = 5;
    [SerializeField] private float imageSpacing = 0.05f;

    //Distance between two ActionBarUI slider values. First in the order sits at the slider max.
    private float imageInterval;
    private float sliderMin;
    private float sliderMax;

    private readonly List<ActionBarUnit> units = new();
    private readonly List<Slider> actionBarUIs = new();
    private ActionBarUnit activeUnit;

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<KillEnemyGA>(OnEnemyKilled, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<KillHeroGA>(OnHeroKilled, ReactionTiming.PRE);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<KillEnemyGA>(OnEnemyKilled, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<KillHeroGA>(OnHeroKilled, ReactionTiming.PRE);
    }

    //Builds a unit for every hero and enemy already spawned, then starts the first turn.
    public void BeginBattle()
    {
        units.Clear();
        activeUnit = null;

        foreach (HeroView hero in HeroSystem.Instance.Heroes)
        {
            units.Add(new ActionBarUnit(hero));
        }

        foreach (EnemyView enemy in EnemySystem.Instance.Enemies)
        {
            units.Add(new ActionBarUnit(enemy));
        }

        CalculateImageInterval();
        RebuildActionBars();
        BeginActiveTurn();
    }

    //The end turn button calls this. Enemy turns ignore it and advance on their own.
    public void EndHeroTurn()
    {
        if (ActionSystem.Instance.IsStopped)
        {
            return;
        }

        if (activeUnit == null || !activeUnit.IsHero)
        {
            return;
        }

        if (ActionSystem.Instance.IsPerforming)
        {
            return;
        }

        HeroTurnEndGA heroTurnEndGA = new(activeUnit.Hero);
        ActionSystem.Instance.Perform(heroTurnEndGA, CompleteActiveTurn);
    }

    //The unit with the lowest action value is the one whose turn it is.
    private void BeginActiveTurn()
    {
        if (ActionSystem.Instance.IsStopped)
        {
            return;
        }

        activeUnit = GetLowestUnit(null);
        if (activeUnit == null)
        {
            return;
        }

        if (activeUnit.IsHero)
        {
            HeroTurnStartGA heroTurnStartGA = new(activeUnit.Hero);
            ActionSystem.Instance.Perform(heroTurnStartGA);
            return;
        }

        EnemyActGA enemyActGA = new(activeUnit.Enemy);
        ActionSystem.Instance.Perform(enemyActGA, CompleteActiveTurn);
    }

    //At the end of a turn, time advances by the acting unit's action value, then that unit waits a full base action value again.
    private void CompleteActiveTurn()
    {
        if (ActionSystem.Instance.IsStopped)
        {
            return;
        }

        if (activeUnit != null)
        {
            //A unit killed on their own turn is already off the roster, but time still advances by the action value they spent.
            if (units.Contains(activeUnit))
            {
                AdvanceActionValues(activeUnit);
            }
            else
            {
                float elapsed = activeUnit.ActionValue;
                foreach (ActionBarUnit unit in units)
                {
                    unit.ActionValue -= elapsed;
                }
            }
        }

        activeUnit = null;
        RemoveFrontActionBar();
        AddUpcomingActionBar();
        BeginActiveTurn();
    }

    private void AdvanceActionValues(ActionBarUnit actingUnit)
    {
        float elapsed = actingUnit.ActionValue;
        foreach (ActionBarUnit unit in units)
        {
            unit.ActionValue -= elapsed;
        }

        actingUnit.ActionValue = actingUnit.BaseActionValue;
    }

    private void OnEnemyKilled(KillEnemyGA killEnemyGA)
    {
        ActionBarUnit killedUnit = null;
        foreach (ActionBarUnit unit in units)
        {
            if (unit.Enemy == killEnemyGA.EnemyView)
            {
                killedUnit = unit;
                break;
            }
        }

        if (killedUnit == null)
        {
            return;
        }

        units.Remove(killedUnit);
        RebuildActionBars();
    }

    private void OnHeroKilled(KillHeroGA killHeroGA)
    {
        ActionBarUnit killedUnit = null;
        foreach (ActionBarUnit unit in units)
        {
            if (unit.Hero == killHeroGA.HeroView)
            {
                killedUnit = unit;
                break;
            }
        }

        if (killedUnit == null)
        {
            return;
        }

        units.Remove(killedUnit);
        RebuildActionBars();
    }

    //imageSpacing is the buffer left below the last portrait. imageInterval is the gap between two portraits.
    private void CalculateImageInterval()
    {
        if (!ActionBarUICreator.Instance.HasReferences)
        {
            imageInterval = 0f;
            return;
        }

        sliderMin = ActionBarUICreator.Instance.MinValue;
        sliderMax = ActionBarUICreator.Instance.MaxValue;

        int gaps = maxActionBarUIs - 1;
        if (gaps <= 0)
        {
            imageInterval = 0f;
            return;
        }

        imageInterval = Mathf.Max(0f, (sliderMax - sliderMin - imageSpacing) / gaps);
    }

    private void RebuildActionBars()
    {
        ClearActionBars();
        if (!CanBuildBars())
        {
            return;
        }

        List<ActionBarUnit> forecast = BuildForecast();
        int count = Mathf.Min(maxActionBarUIs, forecast.Count);
        for (int i = 0; i < count; i++)
        {
            CreateActionBar(forecast[i]);
        }

        RefreshBarValues();
    }

    private void RemoveFrontActionBar()
    {
        if (actionBarUIs.Count == 0)
        {
            return;
        }

        Slider front = actionBarUIs[0];
        actionBarUIs.RemoveAt(0);
        Destroy(front.gameObject);
        Debug.Log("Remove Latest ActionBarUI");
    }

    private void AddUpcomingActionBar()
    {
        if (!CanBuildBars())
        {
            return;
        }

        List<ActionBarUnit> forecast = BuildForecast();
        if (forecast.Count <= actionBarUIs.Count)
        {
            RefreshBarValues();
            return;
        }

        CreateActionBar(forecast[actionBarUIs.Count]);
        RefreshBarValues();
        Debug.Log("Adding new ActionBarUI");
    }

    //Current actor first, then each later turn. The same unit can appear twice when their speed earns another turn before someone else acts.
    private List<ActionBarUnit> BuildForecast()
    {
        List<ActionBarUnit> forecast = new();
        Dictionary<ActionBarUnit, float> simulated = CopyActionValues();

        if (activeUnit != null)
        {
            forecast.Add(activeUnit);

            //Keep the acting unit on the bar until their turn ends, even if they died during it.
            bool stillInBattle = simulated.ContainsKey(activeUnit);
            if (!stillInBattle)
            {
                simulated.Add(activeUnit, activeUnit.ActionValue);
            }

            ApplyTurnAdvance(simulated, activeUnit);

            if (!stillInBattle)
            {
                simulated.Remove(activeUnit);
            }
        }

        while (forecast.Count < maxActionBarUIs)
        {
            ActionBarUnit next = GetLowestUnit(simulated);
            if (next == null)
            {
                break;
            }

            forecast.Add(next);
            ApplyTurnAdvance(simulated, next);
        }

        return forecast;
    }

    private Dictionary<ActionBarUnit, float> CopyActionValues()
    {
        Dictionary<ActionBarUnit, float> simulated = new();
        foreach (ActionBarUnit unit in units)
        {
            if (!IsUnitAlive(unit))
            {
                continue;
            }

            simulated.Add(unit, unit.ActionValue);
        }

        return simulated;
    }

    private void ApplyTurnAdvance(Dictionary<ActionBarUnit, float> simulated, ActionBarUnit actingUnit)
    {
        float elapsed = simulated[actingUnit];
        List<ActionBarUnit> keys = new(simulated.Keys);
        foreach (ActionBarUnit unit in keys)
        {
            simulated[unit] -= elapsed;
        }

        simulated[actingUnit] = actingUnit.BaseActionValue;
    }

    //Passing null reads the live action values. A dictionary reads a simulated future.
    private ActionBarUnit GetLowestUnit(Dictionary<ActionBarUnit, float> simulated)
    {
        ActionBarUnit lowestUnit = null;
        float lowestValue = float.MaxValue;

        if (simulated == null)
        {
            foreach (ActionBarUnit unit in units)
            {
                if (!IsUnitAlive(unit))
                {
                    continue;
                }

                if (unit.ActionValue < lowestValue)
                {
                    lowestValue = unit.ActionValue;
                    lowestUnit = unit;
                }
            }

            return lowestUnit;
        }

        foreach (KeyValuePair<ActionBarUnit, float> pair in simulated)
        {
            if (pair.Value < lowestValue)
            {
                lowestValue = pair.Value;
                lowestUnit = pair.Key;
            }
        }

        return lowestUnit;
    }

    private bool IsUnitAlive(ActionBarUnit unit)
    {
        return unit.View != null && unit.View.CurrentHealth > 0;
    }

    private bool CanBuildBars()
    {
        if (ActionBarUICreator.Instance.HasReferences)
        {
            return true;
        }

        Debug.LogError("ActionBarUICreator is missing the ActionBarUI prefab or the ActionBarsUI parent.");
        return false;
    }

    private void CreateActionBar(ActionBarUnit unit)
    {
        Slider bar = ActionBarUICreator.Instance.CreateActionBarUI(unit.View.ActionBarImage);
        actionBarUIs.Add(bar);
    }

    private void RefreshBarValues()
    {
        for (int i = 0; i < actionBarUIs.Count; i++)
        {
            actionBarUIs[i].value = SliderValueForTurnIndex(i);
        }

        //The unit whose turn it is draws on top of the portraits behind them.
        for (int i = actionBarUIs.Count - 1; i >= 0; i--)
        {
            actionBarUIs[i].transform.SetAsLastSibling();
        }
    }

    private float SliderValueForTurnIndex(int index)
    {
        float value = sliderMax - index * imageInterval;
        if (value < sliderMin)
        {
            return sliderMin;
        }

        return value;
    }

    private void ClearActionBars()
    {
        foreach (Slider bar in actionBarUIs)
        {
            if (bar != null)
            {
                Destroy(bar.gameObject);
            }
        }

        actionBarUIs.Clear();
    }

    private sealed class ActionBarUnit
    {
        public CombatantView View { get; }
        public float ActionValue { get; set; }
        public float BaseActionValue => View.BaseActionValue;
        public bool IsHero => View is HeroView;
        public HeroView Hero => View as HeroView;
        public EnemyView Enemy => View as EnemyView;

        public ActionBarUnit(CombatantView view)
        {
            View = view;
            ActionValue = BaseActionValue;
        }
    }
}
