using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//RESPONSIBLE FOR PROVIDING THE CORRECT TARGETS
public abstract class TargetMode
{
    public abstract List<CombatantView> GetTargets();
}
