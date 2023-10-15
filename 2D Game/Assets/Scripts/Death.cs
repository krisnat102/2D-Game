using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    [SerializeField] private float animationLength = 1f;

    public void Start()
    {
        Invoke("DeathAnimation", animationLength);
    }

    public void DeathAnimation()
    {
        Destroy(gameObject);
    }

    
}
