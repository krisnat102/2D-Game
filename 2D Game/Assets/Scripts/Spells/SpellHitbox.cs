using UnityEngine;
using System;

public class SpellHitbox : MonoBehaviour
{
    [SerializeField] private Spell spell;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private GameObject impactEffect;

    [SerializeField] private Animator anim;
    void Start()
    {
        rb.velocity = transform.right * spell.speed;
    }

    private void Update()
    {
        if (GameManager.gamePaused == false)
        {
            Vector3 range = new Vector3(spell.range, spell.range, 0);
            if (Math.Abs(Abilities.castPoint.x) + range.x < Math.Abs(transform.position.x) || Math.Abs(Abilities.castPoint.y) + range.y > Math.Abs(transform.position.y))
            {
                Invoke("DestroyObject", 0.5f);

                anim.SetBool("End", true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag != "Player" && hitInfo.tag != "Item")
        {
            Enemy enemy = hitInfo.GetComponent<Enemy>();
            if (enemy == true)
            {
                enemy.TakeDamage(spell.value);
            }
            Instantiate(impactEffect, transform.position, transform.rotation);

            DestroyObject();
        }
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
