using UnityEngine;
using Pathfinding;

public class EnemyGFX : MonoBehaviour
{

    public AIPath aiPath;

    [SerializeField] private float sizeX = 1f;
    [SerializeField] private float sizeY = 1f;

    void Update()
    {
        if (Core.GameManager.Instance.gamePaused == false)
        {
            if (aiPath.desiredVelocity.x >= 0.01f)
            {
                transform.localScale = new Vector3(-sizeX, sizeY, 1f);
            }
            else if (aiPath.desiredVelocity.x <= 0.01f)
            {
                transform.localScale = new Vector3(sizeX, sizeY, 1f);
            }
        }
    }
}
