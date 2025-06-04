using System;
using UnityEngine;

namespace Bardent.Weapons.Components
{
    [Serializable]
    public class AttackActionHitBox : AttackData
    {
        public bool Debug;
        [field: SerializeField] public float Delay { get; private set; }
        [field: SerializeField] public Rect HitBox { get; private set; }
    }
}