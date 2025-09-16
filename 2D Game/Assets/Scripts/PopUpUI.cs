using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class PopUpUI : MonoBehaviour
    {
        public void GoUp() => transform.LeanMove(new Vector2(transform.position.x, transform.position.y + UIManager.instance.MoveDistance), UIManager.instance.MoveDuration);
    }
}
