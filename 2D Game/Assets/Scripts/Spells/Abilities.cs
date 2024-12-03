using UnityEngine;
using UnityEngine.UI;
using Bardent.CoreSystem;

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
        private Spell lastCastSpell;
        private Spell lastCastAbility;
        private PlayerData playerData;
        private SpellManager spellManager;
        private PlayerInputHandler playerInputHandler;
        #endregion

        #region Properties
        public bool AbilityCooldown1 { get => abilityCooldown; set => abilityCooldown = value; }
        #endregion

        #region Unity Methods
        void Start()
        {
            playerData = transform.GetComponent<Player>().PlayerData;
            spellManager = SpellManager.Instance;
            playerInputHandler = PlayerInputHandler.Instance;
        }
        private void Update()
        {
            if (playerInputHandler.NormInputX < 0)
            {
                side = false;
            }
            if (playerInputHandler.NormInputX > 0)
            {
                side = true;
            }

            #region Spell Casting
            if (spellManager.SpellsBar.Count > activeSpell)
            {
                if (playerInputHandler.SpellInput && Stats.Instance.Mana.CurrentValue >= spellManager.SpellsBar[activeSpell].cost && spellCooldown == false)
                    Spell();
            }

            if (playerInputHandler.SwitchSpell1Input)
            {
                playerInputHandler.UseSwitchSpell1Input();

                if (activeSpell != 0 && spellManager.SpellsBar.Count >= activeSpell)
                {
                    activeSpell--;
                }
                else if (activeSpell == 0)
                {
                    activeSpell = 7;
                }
            }
            ClearSprites();

            if (playerInputHandler.SwitchSpell2Input)
            {
                playerInputHandler.UseSwitchSpell2Input();

                if (activeSpell != 7 && activeSpell < spellManager.SpellsBar.Count)
                {
                    activeSpell++;
                }
                else if(activeSpell == 7)
                {
                    activeSpell = 0;
                }
            }
            ClearSprites();
            #endregion

            #region Ability Casting
            if (spellManager.AbilitiesBar.Count > activeAbility)
            {
                if (playerInputHandler.AbilityInput && Stats.Instance.Stam.CurrentValue >= spellManager.AbilitiesBar[activeAbility].cost && AbilityCooldown1 == false)
                    Ability();
            }

            if (playerInputHandler.SwitchAbility1Input)
            {
                playerInputHandler.UseSwitchAbility1Input();
                
                if (activeAbility != 0 && spellManager.AbilitiesBar.Count >= activeAbility)
                {
                    activeAbility--;
                }
                else if (activeAbility == 0)
                {
                    activeAbility = 7;
                }
            }
            ClearSprites();

            if (playerInputHandler.SwitchAbility2Input)
            {
                playerInputHandler.UseSwitchAbility2Input();

                if (activeAbility != 7 && activeAbility < spellManager.AbilitiesBar.Count)
                {
                    activeAbility++;
                }
                else if (activeAbility == 7)
                {
                    activeAbility = 0;
                }
            }
            ClearSprites();
            #endregion

            #region Spells UI
            if (spellManager.SpellsBar.Count > activeSpell)
                if (spellManager.SpellsBar[activeSpell] != null)
                {
                    mainSpell.sprite = spellManager.SpellsBar[activeSpell]?.icon;
                    mainSpell.gameObject.SetActive(true);
                }

            if (spellManager.SpellsBar.Count > activeSpell - 1 && activeSpell != 0)
            {
                if (spellManager.SpellsBar[activeSpell - 1] != null && spellManager.SpellsBar.Count > activeSpell - 1)
                {
                    sideSpell2.sprite = spellManager.SpellsBar[activeSpell - 1]?.icon;
                    sideSpell2.gameObject.SetActive(true);
                }
            }
            else if (spellManager.SpellsBar.Count == 7 && activeSpell == 0)
            {
                sideSpell2.sprite = spellManager.SpellsBar[7]?.icon;
                sideSpell2.gameObject.SetActive(true);
            }

            if (spellManager.SpellsBar.Count > activeSpell + 1 && activeSpell != 7)
            {
                if (spellManager.SpellsBar[activeSpell + 1] != null && spellManager.SpellsBar.Count > activeSpell + 1)
                {
                    sideSpell1.sprite = spellManager.SpellsBar[activeSpell + 1]?.icon;
                    sideSpell1.gameObject.SetActive(true);
                }
            }
            else if (spellManager.SpellsBar.Count > 0 && activeSpell == 7)
            {
                sideSpell1.sprite = spellManager.SpellsBar[0]?.icon;
                sideSpell1.gameObject.SetActive(true);
            }
            #endregion

            #region Abilities UI
            if (spellManager.AbilitiesBar.Count > activeAbility)
                if (spellManager.AbilitiesBar[activeAbility] != null)
                {
                    mainAbility.sprite = spellManager.AbilitiesBar[activeAbility]?.icon;
                    mainAbility.gameObject.SetActive(true);
                }

            if (spellManager.AbilitiesBar.Count > activeAbility - 1 && activeAbility != 0)
            {
                if (spellManager.AbilitiesBar[activeAbility - 1] != null && spellManager.AbilitiesBar.Count > activeAbility - 1)
                {
                    sideAbility2.sprite = spellManager.AbilitiesBar[activeAbility - 1]?.icon;
                    sideAbility2.gameObject.SetActive(true);
                }
            }
            else if (spellManager.AbilitiesBar.Count == 7 && activeAbility == 0)
            {
                sideAbility2.sprite = spellManager.AbilitiesBar[7]?.icon;
                sideAbility2.gameObject.SetActive(true);
            }

            if (spellManager.AbilitiesBar.Count > activeAbility + 1 && activeAbility != 7)
            {
                if (spellManager.AbilitiesBar[activeAbility + 1] != null && spellManager.AbilitiesBar.Count > activeAbility + 1)
                {
                    sideAbility1.sprite = spellManager.AbilitiesBar[activeAbility + 1]?.icon;
                    sideAbility1.gameObject.SetActive(true);
                }
            }
            else if (spellManager.AbilitiesBar.Count > 0 && activeAbility == 7)
            {
                sideAbility1.sprite = spellManager.AbilitiesBar[0]?.icon;
                sideAbility1.gameObject.SetActive(true);
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
            if (spellManager.SpellsBar[activeSpell] != null && Stats.Instance.Mana.CurrentValue >= spellManager.SpellsBar[activeSpell].cost)
            {
                //Vector2 offset = new Vector2(OffsetX, OffsetY);
                if (spellManager.SpellsBar[activeSpell].useOffset)
                {
                    if (side)
                    {
                        newPosition = (Vector2)castingPoint.position + spellManager.SpellsBar[activeSpell].offset;
                    }
                    else
                    {
                        newPosition = (Vector2)castingPoint.position + new Vector2(-spellManager.SpellsBar[activeSpell].offset.x, spellManager.SpellsBar[activeSpell].offset.y);
                    }
                    //new ObjectPool(spellsBar[activeSpell].spellEffect, newPosition, castingPoint.rotation, 5, 10);
                    Instantiate(spellManager.SpellsBar[activeSpell].spellEffect, newPosition, castingPoint.rotation);
                }
                else
                {
                    Instantiate(spellManager.SpellsBar[activeSpell].spellEffect, castingPoint.position, castingPoint.rotation);
                    //new ObjectPool(spellsBar[activeSpell].spellEffect, castingPoint.position, castingPoint.rotation, 5, 10);
                }

                Stats.Instance.Mana.CurrentValue -= spellManager.SpellsBar[activeSpell].cost;

                spellCooldown = true;
                lastCastSpell = spellManager.SpellsBar[activeSpell];
                Invoke("SpellCooldown", spellManager.SpellsBar[activeSpell].cooldown);
                spellCooldownImg.gameObject.SetActive(true);
                spellCooldownImg.fillAmount = 1;
            }
        }
        void Ability()
        {
            Grappler grappler = gameObject.GetComponent<Grappler>();

            if (spellManager.AbilitiesBar[activeAbility] != null)
            {
                if (spellManager.AbilitiesBar[activeAbility].name == "grappling hook")
                {
                    grappler.enabled = true;

                    AbilityCooldown1 = true;
                    Invoke("AbilityCooldown", spellManager.AbilitiesBar[activeAbility].cooldown);
                    abilityCooldownImg.gameObject.SetActive(true);
                    abilityCooldownImg.fillAmount = 1;
                }
                else if (Stats.Instance.Stam.CurrentValue >= spellManager.AbilitiesBar[activeAbility].cost)
                {
                    //Vector2 offset = new Vector2(OffsetX2, OffsetY2);

                    Instantiate(spellManager.AbilitiesBar[activeAbility].spellEffect, castingPoint.position, castingPoint.rotation);

                    Stats.Instance.Stam.CurrentValue -= spellManager.AbilitiesBar[activeAbility].cost;
                    Stats.Instance.Stam.StopRegen(playerData.stamRecoveryTime);

                    AbilityCooldown1 = true;
                    lastCastAbility = spellManager.AbilitiesBar[activeAbility];
                    Invoke("AbilityCooldown", spellManager.AbilitiesBar[activeAbility].cooldown);
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
