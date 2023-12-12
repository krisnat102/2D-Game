using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyAIData", menuName = "Data/Enemy Data /AI Data")]
public class EnemyDataAI : ScriptableObject
{
    [Header("AI Pathfinding")]
    public float activateDistance = 50f;
    public float pathUpdateTime = 0.5f;

    [Header("AI Physics")]
    public float speed = 2000f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;

    [Header("Custom Behaviour")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;
}
