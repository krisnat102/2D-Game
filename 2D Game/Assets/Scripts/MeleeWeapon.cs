using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{

    public GameObject hitbox;

    public Transform trans;

    [SerializeField] private float attackSpeed = 1.3f;
    private float nextAttackTime = 0.0f;
    [SerializeField] private float offsetX = 0.3f;
    [SerializeField] private float offsetY = 0f;


    void Start()
    {
        
    }

    void Update()
    {
        if (GameManager.gamePaused == false)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
            }
        }
    }
    
    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1.0f / attackSpeed;

            FindObjectOfType<AudioManager>().Play("SwordAttack");

            if (MeleeWeaponSprite.side == true)
            {
                Vector3 offset = new Vector3(offsetX, offsetY, 0);

                Instantiate(hitbox, trans.position + offset, trans.rotation);
            }
            else
            {
                Vector3 offset = new Vector3(-offsetX, offsetY, 0);

                Instantiate(hitbox, trans.position + offset, trans.rotation);
            }
        }

    }
}
