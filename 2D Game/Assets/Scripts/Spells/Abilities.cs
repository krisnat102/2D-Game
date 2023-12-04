using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{

    [SerializeField] private Spell ability;

    [SerializeField] private Transform castingPoint;

    [Header("Offsets")]
    [SerializeField] private float OffsetX;
    [SerializeField] private float OffsetY;
    [SerializeField] private float OffsetX2;
    [SerializeField] private float OffsetY2;

    [Header("Active Spell Holders")]
    [SerializeField] private Image mainSpell;
    [SerializeField] private Image sideSpell1;
    [SerializeField] private Image sideSpell2;

    private bool spellCooldown = false;
    private bool abilityCooldown = false;
    private int activeSpell = 0;

    public static Vector3 castPoint = new Vector3();


    private void Update()
    {
        if (GameManager.gamePaused == false)
        {
            if(SpellManager.SpellsBar.Count >= activeSpell)
                if (Input.GetButtonDown("Spell") && PlayerStats.mana >= SpellManager.SpellsBar[activeSpell].cost && spellCooldown == false)
                    Spell();

            if (Input.GetButtonDown("Ability") && PlayerStats.stam >= ability.cost && abilityCooldown == false)
                Ability();

            if (Input.GetButtonDown("ChangeSpell2"))
                if (activeSpell != 0)
                {
                    activeSpell--;
                }
                else activeSpell = 7;
                ClearSprites();

            if (Input.GetButtonDown("ChangeSpell1"))
                if (activeSpell != 7)
                {
                    activeSpell++;
                }
                else activeSpell = 0;
                ClearSprites();
        }

        if (SpellManager.SpellsBar.Count > activeSpell)
            if (SpellManager.SpellsBar[activeSpell] != null)
            {
                mainSpell.sprite = SpellManager.SpellsBar[activeSpell].icon;
                mainSpell.gameObject.SetActive(true);
            }

        if (SpellManager.SpellsBar.Count > activeSpell - 1 && activeSpell != 0)
        {
            if (SpellManager.SpellsBar[activeSpell - 1] != null && SpellManager.SpellsBar.Count > activeSpell - 1)
            {
                sideSpell1.sprite = SpellManager.SpellsBar[activeSpell - 1].icon;
                sideSpell1.gameObject.SetActive(true);
            }
        }
        else if (SpellManager.SpellsBar.Count == 7)
        {
            sideSpell1.sprite = SpellManager.SpellsBar[7].icon;
            sideSpell1.gameObject.SetActive(true);
        }

        if (SpellManager.SpellsBar.Count > activeSpell + 1 && activeSpell != 7)
        {
            if (SpellManager.SpellsBar[activeSpell + 1] != null && SpellManager.SpellsBar.Count > activeSpell + 1)
            {
                sideSpell2.sprite = SpellManager.SpellsBar[activeSpell + 1].icon;
                sideSpell2.gameObject.SetActive(true);
            }
        }
        else if (SpellManager.SpellsBar.Count == 7)
        {
            sideSpell2.sprite = SpellManager.SpellsBar[0].icon;
            sideSpell2.gameObject.SetActive(true);
        }
    }

    void Spell()
    {
        if (SpellManager.SpellsBar[activeSpell] != null)
        {
            Vector2 offset = new Vector2(OffsetX, OffsetY);

            Instantiate(SpellManager.SpellsBar[activeSpell].spellEffect, castingPoint.position, castingPoint.rotation);

            castPoint = castingPoint.position;

            PlayerStats.mana -= SpellManager.SpellsBar[activeSpell].cost;

            spellCooldown = true;
            Invoke("SpellCooldown", SpellManager.SpellsBar[activeSpell].cooldown);
        }
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

    void ClearSprites()
    {
        mainSpell.sprite = null;
        sideSpell1.sprite = null;
        sideSpell2.sprite = null;

        mainSpell.gameObject.SetActive(false);
        sideSpell1.gameObject.SetActive(false);
        sideSpell2.gameObject.SetActive(false);
    }
}
    