using UnityEngine;
using UnityEngine.UI;
using Bardent.CoreSystem;
using Krisnat;

namespace Spells
{
    public class Abilities : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Transform castingPoint;

        [Header("Active Spell Holders")]
        [SerializeField] private Image mainSpell;
        [SerializeField] private Image sideSpell1;
        [SerializeField] private Image sideSpell2;
        [SerializeField] private Image spellCooldownImg;

        [Header("Active Ability Holders")]
        [SerializeField] private Image mainAbility;
        [SerializeField] private Image sideAbility1;
        [SerializeField] private Image sideAbility2;
        [SerializeField] private Image abilityCooldownImg;

        private bool spellCooldown = false;
        private bool side = true;
        private bool abilityCooldown = false;
        private int activeSpell = 0;
        private int activeAbility = 0;
        private Vector2 newPosition;

        public bool AbilityCooldown1 { get => abilityCooldown; set => abilityCooldown = value; }
        #endregion

        #region Unity Methods
        private void Update()
        {
            if (Core.GameManager.gamePaused == false)
            {
                if (PlayerInputHandler.Instance.NormInputX < 0)
                {
                    side = false;
                }
                if (PlayerInputHandler.Instance.NormInputX > 0)
                {
                    side = true;
                }
                
                #region Spells
                if (SpellManager.SpellsBar.Count >= activeSpell)
                    if (PlayerInputHandler.Instance.SpellInput && Stats.Instance.Mana.CurrentValue >= SpellManager.SpellsBar[activeSpell].cost && spellCooldown == false)
                        Spell();

                if (PlayerInputHandler.Instance.SwitchSpell1Input)
                    if (activeSpell != 0)
                    {
                        activeSpell--;
                        PlayerInputHandler.Instance.UseSwitchSpell1Input();
                    }
                    else activeSpell = 7;
                ClearSprites();

                if (PlayerInputHandler.Instance.SwitchSpell2Input)
                    if (activeSpell != 7)
                    {
                        activeSpell++;
                        PlayerInputHandler.Instance.UseSwitchSpell2Input();
                    }
                    else activeSpell = 0;
                ClearSprites();
                #endregion

                #region Abilites
                if (SpellManager.AbilitiesBar.Count >= activeAbility)
                {
                    if (PlayerInputHandler.Instance.AbilityInput && Stats.Instance.Stam.CurrentValue >= SpellManager.AbilitiesBar[activeAbility].cost && AbilityCooldown1 == false)
                        Ability();
                }
                if (PlayerInputHandler.Instance.SwitchAbility1Input)
                    if (activeAbility != 0)
                    {
                        activeAbility--;
                        PlayerInputHandler.Instance.UseSwitchAbility1Input();
                    }
                    else activeAbility = 7;
                ClearSprites();

                if (PlayerInputHandler.Instance.SwitchAbility2Input)
                    if (activeAbility != 7)
                    {
                        activeAbility++;
                        PlayerInputHandler.Instance.UseSwitchAbility2Input();
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

            if (abilityCooldownImg.gameObject.activeSelf && abilityCooldownImg.fillAmount > 0)
            {
                abilityCooldownImg.fillAmount -= Time.deltaTime / SpellManager.AbilitiesBar[activeAbility].cooldown;
            }
            if (spellCooldownImg.gameObject.activeSelf && spellCooldownImg.fillAmount > 0)
            {
                spellCooldownImg.fillAmount -= Time.deltaTime / SpellManager.SpellsBar[activeSpell].cooldown;
            }
        }
        #endregion

        #region Spell And Ability Casting
        void Spell()
        {
            if (SpellManager.SpellsBar[activeSpell] != null && Stats.Instance.Mana.CurrentValue >= SpellManager.SpellsBar[activeSpell].cost)
            {
                //Vector2 offset = new Vector2(OffsetX, OffsetY);
                if (SpellManager.SpellsBar[activeSpell].useOffset)
                {
                    if (side)
                    {
                        newPosition = (Vector2)castingPoint.position + SpellManager.SpellsBar[activeSpell].offset;
                    }
                    else
                    {
                        newPosition = (Vector2)castingPoint.position + new Vector2(-SpellManager.SpellsBar[activeSpell].offset.x, SpellManager.SpellsBar[activeSpell].offset.y);
                    }
                    Instantiate(SpellManager.SpellsBar[activeSpell].spellEffect, newPosition, castingPoint.rotation);
                }
                else
                {
                    Debug.Log(castingPoint.rotation);
                    Instantiate(SpellManager.SpellsBar[activeSpell].spellEffect, castingPoint.position, castingPoint.rotation);
                }

                Stats.Instance.Mana.CurrentValue -= SpellManager.SpellsBar[activeSpell].cost;

                spellCooldown = true;
                Invoke("SpellCooldown", SpellManager.SpellsBar[activeSpell].cooldown);
                spellCooldownImg.gameObject.SetActive(true);
                spellCooldownImg.fillAmount = 1;
            }
        }
        void Ability()
        {
            Grappler grappler = gameObject.GetComponent<Grappler>();

            if (SpellManager.AbilitiesBar[activeAbility] != null)
            {
                if (SpellManager.AbilitiesBar[activeAbility].name == "grappling hook")
                {
                    grappler.enabled = true;

                    AbilityCooldown1 = true;
                    Invoke("AbilityCooldown", SpellManager.AbilitiesBar[activeAbility].cooldown);
                    abilityCooldownImg.gameObject.SetActive(true);
                    abilityCooldownImg.fillAmount = 1;
                }
                else if(Stats.Instance.Stam.CurrentValue >= SpellManager.AbilitiesBar[activeAbility].cost)
                {
                    //Vector2 offset = new Vector2(OffsetX2, OffsetY2);

                    Instantiate(SpellManager.AbilitiesBar[activeAbility].spellEffect, castingPoint.position, castingPoint.rotation);

                    Stats.Instance.Stam.CurrentValue -= SpellManager.AbilitiesBar[activeAbility].cost;

                    AbilityCooldown1 = true;
                    Invoke("AbilityCooldown", SpellManager.AbilitiesBar[activeAbility].cooldown);
                    abilityCooldownImg.gameObject.SetActive(true);
                    abilityCooldownImg.fillAmount = 1;

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
            AbilityCooldown1 = false;
            abilityCooldownImg.gameObject.SetActive(false);
        }
        #endregion

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
}