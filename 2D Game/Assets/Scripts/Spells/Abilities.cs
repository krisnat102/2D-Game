using UnityEngine;

public class Abilities : MonoBehaviour
{
    public Spell spell;
    public Spell ability;

    [SerializeField] private float OffsetX;
    [SerializeField] private float OffsetY;

    [SerializeField] private float OffsetX2;
    [SerializeField] private float OffsetY2;

    private bool spellCooldown = false;
    private bool abilityCooldown = false;

    public static Vector3 castPoint = new Vector3();

    public Transform castingPoint;

    private void Update()
    {
        if (GameManager.gamePaused == false)
        {
            if (Input.GetButtonDown("Spell") && PlayerStats.mana >= spell.cost && spellCooldown == false)
            {
                Spell();
            }

            if (Input.GetButtonDown("Ability") && PlayerStats.stam >= ability.cost && abilityCooldown == false)
            {
                Ability();
            }
        }
    }

    void Spell()
    {
        Vector2 offset = new Vector2(OffsetX, OffsetY);

        Instantiate(spell.spellEffect, castingPoint.position, castingPoint.rotation);

        castPoint = castingPoint.position;

        PlayerStats.mana -= spell.cost;

        spellCooldown = true;
        Invoke("SpellCooldown", spell.cooldown);
    }

    void Ability()
    {
        Vector2 offset = new Vector2(OffsetX2, OffsetY2);

        Instantiate(ability.spellEffect, castingPoint.position, castingPoint.rotation);

        PlayerStats.stam -= ability.cost;

        abilityCooldown = true;
        Invoke("AbilityCooldown", ability.cooldown);
    }

    private void SpellCooldown()
    {
        spellCooldown = false;
    }
    private void AbilityCooldown()
    {
        abilityCooldown = false;
    }
}
