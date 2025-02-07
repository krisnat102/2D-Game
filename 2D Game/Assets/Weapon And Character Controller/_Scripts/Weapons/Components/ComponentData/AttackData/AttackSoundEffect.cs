using System;
using UnityEngine;

namespace Bardent.Weapons.Components
{
    [Serializable]
    public class AttackSoundEffect : AttackData
    {
        [field: SerializeField] public float Delay { get; private set; }
        [field: SerializeField] public AudioClip SFX { get; private set; }
    }
}