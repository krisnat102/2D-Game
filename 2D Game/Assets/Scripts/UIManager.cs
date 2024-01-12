using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [SerializeField] private GameObject purse;

        private Vector3 oldPosition;

        private void Awake()
        {
            Instance = this;
        }

        public void MovePurseAnimation(bool directionUpOrDown, float animationDistance, float animationDuration)
        {
            if (directionUpOrDown)
            {
                //purse.transform.LeanMoveLocal(new Vector2(purse.transform.position.x, purse.transform.position.y + animationDistance), animationDuration);
                purse.transform.LeanMove(new Vector2(purse.transform.position.x, purse.transform.position.y + animationDistance), animationDuration);
                oldPosition = purse.transform.position;
            }
            else
            {
                purse.transform.LeanMove(oldPosition, animationDuration);
            }
        }
    }
}
