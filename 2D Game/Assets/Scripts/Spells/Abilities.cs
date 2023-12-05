using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{
    [SerializeField] private Transform castingPoint;

    /*[Header("Offsets")]
    [SerializeField] private float OffsetX;
    [SerializeField] private float OffsetY;
    [SerializeField] private float OffsetX2;
    [SerializeField] private float OffsetY2;*/

    [Header("Active Spell Holders")]
    [SerializeField] private Image mainSpell;
    [SerializeField] private Image sideSpell1;
    [SerializeField] private Image sideSpell2;

    [Header("Active Ability Holders")]
    [SerializeField] private Image mainAbility;
    [SerializeField] private Image sideAbility1;
    [SerializeField] private Image sideAbility2;
    private bool spellCooldown = false;
    private bool abilityCooldown = false;
    private int activeSpell = 0;
    private int activeAbility = 0;

    public static Vector3 castPoint = new Vector3();

    private void Update()
    {
        if (GameManager.gamePaused == false)
        {
            #region Spells
            if (SpellManager.SpellsBar.Count >= activeSpell)
                if (Input.GetButtonDown("Spell") && PlayerStats.mana >= SpellManager.SpellsBar[activeSpell].cost && spellCooldown == false)
                    Spell();

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
            #endregion

            #region Abilites
            if (SpellManager.AbilitiesBar.Count >= activeAbility)
                if (Input.GetButtonDown("Ability") && PlayerStats.stam >= SpellManager.AbilitiesBar[activeAbility].cost && abilityCooldown == false)
                    Ability();

            if (Input.GetButtonDown("ChangeAbility2"))
                if (activeAbility != 0)
                {
                    activeAbility--;
                }
                else activeAbility = 7;
            ClearSprites();

            if (Input.GetButtonDown("ChangeAbility1"))
                if (activeAbility != 7)
                {
                    activeAbility++;
                }
                else activeAbility = 0;
            ClearSprites();
            #endregion
        }
        #region Spells
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
        #endregion

        #region Abilities
        if (SpellManager.AbilitiesBar.Count > activeAbility)
            if (SpellManager.AbilitiesBar[activeAbility] != null)
            {
                mainAbility.sprite = SpellManager.AbilitiesBar[activeAbility].icon;
                mainAbility.gameObject.SetActive(true);
            }

        if (SpellManager.AbilitiesBar.Count > activeAbility - 1 && activeAbility != 0)
        {
            if (SpellManager.AbilitiesBar[activeAbility - 1] != null && SpellManager.AbilitiesBar.Count > activeAbility - 1)
            {
                sideAbility1.sprite = SpellManager.AbilitiesBar[activeAbility - 1].icon;
                sideAbility1.gameObject.SetActive(true);
            }
        }
        else if (SpellManager.AbilitiesBar.Count == 7)
        {
            sideAbility1.sprite = SpellManager.AbilitiesBar[7].icon;
            sideAbility1.gameObject.SetActive(true);
        }

        if (SpellManager.AbilitiesBar.Count > activeAbility + 1 && activeAbility != 7)
        {
            if (SpellManager.AbilitiesBar[activeAbility + 1] != null && SpellManager.AbilitiesBar.Count > activeAbility + 1)
            {
                sideAbility2.sprite = SpellManager.AbilitiesBar[activeAbility + 1].icon;
                sideAbility2.gameObject.SetActive(true);
            }
        }
        else if (SpellManager.AbilitiesBar.Count == 7)
        {
            sideAbility2.sprite = SpellManager.AbilitiesBar[0].icon;
            sideAbility2.gameObject.SetActive(true);
        }
        #endregion
    }

    void Spell()
    {
        if (SpellManager.SpellsBar[activeSpell] != null)
        {
            //Vector2 offset = new Vector2(OffsetX, OffsetY);

            Instantiate(SpellManager.SpellsBar[activeSpell].spellEffect, castingPoint.position, castingPoint.rotation);

            castPoint = castingPoint.position;

            PlayerStats.mana -= SpellManager.SpellsBar[activeSpell].cost;

            spellCooldown = true;
            Invoke("SpellCooldown", SpellManager.SpellsBar[activeSpell].cooldown);
        }
    }

    void Ability()
    {
        Grappler grappler = gameObject.GetComponent<Grappler>();

        if (SpellManager.AbilitiesBar[activeAbility] != null)
        {
            if (SpellManager.AbilitiesBar[activeAbility].id == 2)
            {
                grappler.enabled = true;
            }
            else
            {
                //Vector2 offset = new Vector2(OffsetX2, OffsetY2);

                Instantiate(SpellManager.AbilitiesBar[activeAbility].spellEffect, castingPoint.position, castingPoint.rotation);

                PlayerStats.stam -= SpellManager.AbilitiesBar[activeAbility].cost;

                abilityCooldown = true;
                Invoke("AbilityCooldown", SpellManager.AbilitiesBar[activeAbility].cooldown);

                grappler.enabled = false;
            }
        }
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

        mainAbility.sprite = null;
        sideAbility1.sprite = null;
        sideAbility2.sprite = null;

        mainAbility.gameObject.SetActive(false);
        sideAbility1.gameObject.SetActive(false);
        sideAbility2.gameObject.SetActive(false);
    }
}
    