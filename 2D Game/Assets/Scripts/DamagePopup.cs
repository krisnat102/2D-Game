using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private float moveYSpeed;

        void Update()
        {
            transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime; 
        }
    }
}
