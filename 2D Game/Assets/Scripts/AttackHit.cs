using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHit : MonoBehaviour
{

    [SerializeField] private float attackDmg = 20f; 

    void Start()
    {
        Invoke("FinishAttack", 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag == "Player")
        {
            PlayerStats.hp -= attackDmg;

            Debug.Log("it hit");

            Destroy(gameObject);
        }
    }

    private void FinishAttack()
    {
        Destroy(gameObject);
    }
}
