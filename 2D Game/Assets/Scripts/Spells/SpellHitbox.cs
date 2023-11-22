using UnityEngine;
using System;

public class SpellHitbox : MonoBehaviour
{
    [SerializeField] private Spell spell;

    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();


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

                if(ContainsParam(anim, "End")) anim.SetBool("End", true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag != "Player" && hitInfo.tag != "Item" && hitInfo.tag != "Climbable" && hitInfo.tag != "AttackRange" && hitInfo.tag != "BackgroundObject")
        {
            Enemy enemy = hitInfo.GetComponent<Enemy>();
            if (enemy == true)
            {
                enemy.TakeDamage(spell.value);
            }

            if (spell.spellDeath != null) Instantiate(spell.spellDeath, transform.position, Quaternion.identity);

            DestroyObject();
        }
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private bool ContainsParam(Animator _Anim, string _ParamName)
    {
        foreach (AnimatorControllerParameter param in _Anim.parameters)
        {
            if (param.name == _ParamName) return true;
        }
        return false;
    }
}
