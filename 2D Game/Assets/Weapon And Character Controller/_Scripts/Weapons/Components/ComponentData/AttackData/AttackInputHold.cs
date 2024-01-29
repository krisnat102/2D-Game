using Bardent.Weapons.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    [Serializable]
    public class AttackInputHold : AttackData
    {
        [field: SerializeField] public InputHoldAttackData[] inputHoldAttackDatas {  get; private set; }
        [field: SerializeField] public float FirstStageChargeTime { get; private set; }
        [field: SerializeField] public float SecondStageChargeTime { get; private set; }
        [field: SerializeField] public float ThirdStageChargeTime { get; private set; }
        [field: SerializeField] public float FinalStageChargeTime { get; private set; }
    }

    [Serializable]
    public struct InputHoldAttackData
    {
        [field: SerializeField] public Vector2 Offset { get; private set; }
        [field: SerializeField] public Arrow Projectile { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
    }
}
