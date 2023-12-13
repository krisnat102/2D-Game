using UnityEngine;
using Pathfinding;
using Core;

public class EnemyGFX : MonoBehaviour
{

    public AIPath aiPath;

    [SerializeField] private float sizeX = 1f;
    [SerializeField] private float sizeY = 1f;

    void Update()
    {
        if (GameManager.gamePaused == false)
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
