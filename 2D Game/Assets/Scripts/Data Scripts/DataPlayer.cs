using UnityEngine;

[CreateAssetMenu(fileName = "newDataForPlayer", menuName = "Data/Data For Player /Base Data")]
public class DataPlayer : ScriptableObject
{
    [Header("Move")]
    public float runSpeed = 40f;

    [Header("Dodge")]
    public float dodgePower = 200f;
    public float dodgeCost = 20f;
    public float dodgeCooldown = 1f;

    [Header("Climb")]
    public float climbSpeed = 200f;
}