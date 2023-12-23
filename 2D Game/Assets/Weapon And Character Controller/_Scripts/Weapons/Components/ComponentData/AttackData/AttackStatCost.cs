using System;
using System.Collections;
using UnityEngine;

namespace Bardent.Weapons.Components
{
    [Serializable]
    public class AttackStatCost : AttackData
    {
        [field: SerializeField] public float AttackCost { get; private set; }
        [field: SerializeField] public float AttackRecoveryTime { get; private set; }
        [field: SerializeField] public bool StamCost { get; private set; }
    }
}