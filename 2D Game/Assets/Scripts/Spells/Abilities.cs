using UnityEngine;
using UnityEngine.UI;
using Bardent.CoreSystem;
using Krisnat;
using System.Collections.Generic;

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
        private List<Spell> spellsBar;
        private List<Spell> abilitiesBar;
        private Spell lastCastSpell;
        private Spell lastCastAbility;
        private PlayerData playerData;

        public bool AbilityCooldown1 { get => abilityCooldown; set => abilityCooldown = value; }
        #endregion

        #region Unity Methods
        void Start()
        {
            playerData = transform.GetComponent<Player>().PlayerData;
        }
        private void Update()
        {
            spellsBar = SpellManager.Instance.SpellsBar;
            abilitiesBar = SpellManager.Instance.AbilitiesBar;

            if (PlayerInputHandler.Instance.NormInputX < 0)
            {
                side = false;
            }
            if (PlayerInputHandler.Instance.NormInputX > 0)
            {
                side = true;
            }

            #region Spell Casting
            if (spellsBar.Count > activeSpell)
            {
                if (PlayerInputHandler.Instance.SpellInput && Stats.Instance.Mana.CurrentValue >= spellsBar[activeSpell].cost && spellCooldown == false)
                    Spell();
            }
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

            #region Ability Casting
            if (abilitiesBar.Count > activeAbility)
            {
                if (PlayerInputHandler.Instance.AbilityInput && Stats.Instance.Stam.CurrentValue >= abilitiesBar[activeAbility].cost && AbilityCooldown1 == false)
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

            #region Spells UI
            if (spellsBar.Count > activeSpell)
                if (spellsBar[activeSpell] != null)
                {
                    mainSpell.sprite = spellsBar[activeSpell].icon;
                    mainSpell.gameObject.SetActive(true);
                }

            if (spellsBar.Count > activeSpell - 1 && activeSpell != 0)
            {
                if (spellsBar[activeSpell - 1] != null && spellsBar.Count > activeSpell - 1)
                {
                    sideSpell1.sprite = spellsBar[activeSpell - 1].icon;
                    sideSpell1.gameObject.SetActive(true);
                }
            }
            else if (spellsBar.Count == 7)
            {
                sideSpell1.sprite = spellsBar[7].icon;
                sideSpell1.gameObject.SetActive(true);
            }

            if (spellsBar.Count > activeSpell + 1 && activeSpell != 7)
            {
                if (spellsBar[activeSpell + 1] != null && spellsBar.Count > activeSpell + 1)
                {
                    sideSpell2.sprite = spellsBar[activeSpell + 1].icon;
                    sideSpell2.gameObject.SetActive(true);
                }
            }
            else if (spellsBar.Count == 7)
            {
                sideSpell2.sprite = spellsBar[0].icon;
                sideSpell2.gameObject.SetActive(true);
            }
            #endregion

            #region Abilities UI
            if (abilitiesBar.Count > activeAbility)
                if (abilitiesBar[activeAbility] != null)
                {
                    mainAbility.sprite = abilitiesBar[activeAbility].icon;
                    mainAbility.gameObject.SetActive(true);
                }

            if (abilitiesBar.Count > activeAbility - 1 && activeAbility != 0)
            {
                if (abilitiesBar[activeAbility - 1] != null && abilitiesBar.Count > activeAbility - 1)
                {
                    sideAbility1.sprite = abilitiesBar[activeAbility - 1].icon;
                    sideAbility1.gameObject.SetActive(true);
                }
            }
            else if (abilitiesBar.Count == 7)
            {
                sideAbility1.sprite = abilitiesBar[7].icon;
                sideAbility1.gameObject.SetActive(true);
            }

            if (abilitiesBar.Count > activeAbility + 1 && activeAbility != 7)
            {
                if (abilitiesBar[activeAbility + 1] != null && abilitiesBar.Count > activeAbility + 1)
                {
                    sideAbility2.sprite = abilitiesBar[activeAbility + 1].icon;
                    sideAbility2.gameObject.SetActive(true);
                }
            }
            else if (abilitiesBar.Count == 7)
            {
                sideAbility2.sprite = abilitiesBar[0].icon;
                sideAbility2.gameObject.SetActive(true);
            }
            #endregion

            #region Cooldowns
            if (spellCooldown && spellCooldownImg.gameObject.activeSelf && spellCooldownImg.fillAmount > 0)
            {
                spellCooldownImg.fillAmount -= Time.deltaTime / lastCastSpell.cooldown;
            }
            if (abilityCooldown && abilityCooldownImg.gameObject.activeSelf && abilityCooldownImg.fillAmount > 0)
            {
                abilityCooldownImg.fillAmount -= Time.deltaTime / lastCastAbility.cooldown;
            }
            #endregion
        }
        #endregion

        #region Spell And Ability Casting Methods
        void Spell()
        {
            if (spellsBar[activeSpell] != null && Stats.Instance.Mana.CurrentValue >= spellsBar[activeSpell].cost)
            {
                //Vector2 offset = new Vector2(OffsetX, OffsetY);
                if (spellsBar[activeSpell].useOffset)
                {
                    if (side)
                    {
                        newPosition = (Vector2)castingPoint.position + spellsBar[activeSpell].offset;
                    }
                    else
                    {
                        newPosition = (Vector2)castingPoint.position + new Vector2(-spellsBar[activeSpell].offset.x, spellsBar[activeSpell].offset.y);
                    }
                    //new ObjectPool(spellsBar[activeSpell].spellEffect, newPosition, castingPoint.rotation, 5, 10);
                    Instantiate(spellsBar[activeSpell].spellEffect, newPosition, castingPoint.rotation);
                }
                else
                {
                    Instantiate(spellsBar[activeSpell].spellEffect, castingPoint.position, castingPoint.rotation);
                    //new ObjectPool(spellsBar[activeSpell].spellEffect, castingPoint.position, castingPoint.rotation, 5, 10);
                }

                Stats.Instance.Mana.CurrentValue -= spellsBar[activeSpell].cost;

                spellCooldown = true;
                lastCastSpell = spellsBar[activeSpell];
                Invoke("SpellCooldown", spellsBar[activeSpell].cooldown);
                spellCooldownImg.gameObject.SetActive(true);
                spellCooldownImg.fillAmount = 1;
            }
        }
        void Ability()
        {
            Grappler grappler = gameObject.GetComponent<Grappler>();

            if (abilitiesBar[activeAbility] != null)
            {
                if (abilitiesBar[activeAbility].name == "grappling hook")
                {
                    grappler.enabled = true;

                    AbilityCooldown1 = true;
                    Invoke("AbilityCooldown", abilitiesBar[activeAbility].cooldown);
                    abilityCooldownImg.gameObject.SetActive(true);
                    abilityCooldownImg.fillAmount = 1;
                }
                else if (Stats.Instance.Stam.CurrentValue >= abilitiesBar[activeAbility].cost)
                {
                    //Vector2 offset = new Vector2(OffsetX2, OffsetY2);

                    Instantiate(abilitiesBar[activeAbility].spellEffect, castingPoint.position, castingPoint.rotation);

                    Stats.Instance.Stam.CurrentValue -= abilitiesBar[activeAbility].cost;
                    Stats.Instance.Stam.StopRegen(playerData.stamRecoveryTime);

                    AbilityCooldown1 = true;
                    lastCastAbility = abilitiesBar[activeAbility];
                    Invoke("AbilityCooldown", abilitiesBar[activeAbility].cooldown);
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

        #region UI Methods
        public void ClearSprites()
        {
            if (!mainSpell || !sideSpell1 || !sideSpell2 || !mainAbility || !sideAbility1 || !sideAbility2) return;
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
        #endregion
    }
}
