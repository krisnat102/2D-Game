using System;
using UnityEngine;

namespace Bardent.Weapons.Components
{
    [Serializable]
    public class AttackSoundEffect : AttackData
    {
        [field: SerializeField] public float Delay { get; private set; }
        [field: SerializeField] public float LowPitchRange { get; private set; } = 1;
        [field: SerializeField] public float HighPitchRange { get; private set; } = 1;
        [field: SerializeField] public AudioClip[] SFX { get; private set; }
    }
}