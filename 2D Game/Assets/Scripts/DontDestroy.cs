using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class DontDestroy : MonoBehaviour
    {
        private static DontDestroy instance;
        void Awake() 
        {
            // checks if its already triggered
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
