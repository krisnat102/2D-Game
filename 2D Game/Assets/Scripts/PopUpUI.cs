using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class PopUpUI : MonoBehaviour
    {
        public void GoUp()
        {
            transform.LeanMove(new Vector2(transform.position.x, transform.position.y + UIManager.Instance.MoveDistance), UIManager.Instance.MoveDuration);
            Debug.Log(1);
        }
    }
}
