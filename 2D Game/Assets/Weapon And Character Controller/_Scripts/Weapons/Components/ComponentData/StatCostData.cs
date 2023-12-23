using System.Collections;
using UnityEngine;

namespace Bardent.Weapons.Components
{
    public class StatCostData : ComponentData<AttackStatCost>
    {
        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(StatCost);
        }
    }
}