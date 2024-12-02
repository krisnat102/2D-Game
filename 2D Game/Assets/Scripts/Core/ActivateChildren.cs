using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class ActivateChildren : MonoBehaviour
    {
        private void OnEnable()
        {
            Transform[] children = GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
