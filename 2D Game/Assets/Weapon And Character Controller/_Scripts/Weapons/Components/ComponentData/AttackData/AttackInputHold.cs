using Krisnat;
using System;
using UnityEngine;

namespace Bardent.Weapons.Components
{
    [Serializable]
    public class AttackInputHold : AttackData
    {
        [field: SerializeField] public InputHoldAttackData[] inputHoldAttackData {  get; private set; }
        [field: SerializeField] public float AttackCost { get; private set; }
        [field: SerializeField] public float AttackRecoveryTime { get; private set; }
        [field: SerializeField] public bool StamCost { get; private set; }
        [field: SerializeField] public float MinimalHoldTime { get; private set; }
        [field: SerializeField] public float FirstStageChargeTime { get; private set; }
        [field: SerializeField] public float SecondStageChargeTime { get; private set; }
        [field: SerializeField] public float ThirdStageChargeTime { get; private set; }
        [field: SerializeField] public float FinalStageChargeTime { get; private set; }
        [field: SerializeField] public float PerfectShotChargeTimeLowerRange { get; private set; }
        [field: SerializeField] public float PerfectShotChargeTimeUpperRange { get; private set; }
    }

    [Serializable]
    public struct InputHoldAttackData
    {
        [field: SerializeField] public Vector2 Offset { get; private set; }
        [field: SerializeField] public Arrow Projectile { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public int Piercing { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
    }
}
