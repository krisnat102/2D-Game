using UnityEngine;
using UnityEngine.UI;
using Bardent.CoreSystem;

namespace Spells
{
    public class Abilities : MonoBehaviour
    {
        #region Variables
        public static Abilities instance;

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
        public bool AbilityCooldownTracker { get => abilityCooldown; set => abilityCooldown = value; }
        public bool Side { get => side; private set => side = value; }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            instance = this;
        }

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
                Side = false;
            }
            if (playerInputHandler.NormInputX > 0)
            {
                Side = true;
            }

            #region Spell Casting
            //if (IsSpellCastable()) Spell();
            
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
            //if (IsAbilityCastable()) Ability();

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
            if (spellManager.SpellsBar.Count > activeSpell && mainSpell)
                if (spellManager.SpellsBar[activeSpell] != null)
                {
                    mainSpell.sprite = spellManager.SpellsBar[activeSpell]?.icon;
                    mainSpell?.gameObject.SetActive(true);
                }

            if (spellManager.SpellsBar.Count > activeSpell - 1 && activeSpell != 0 && sideSpell2)
            {
                if (spellManager.SpellsBar[activeSpell - 1] != null && spellManager.SpellsBar.Count > activeSpell - 1)
                {
                    sideSpell2.sprite = spellManager.SpellsBar[activeSpell - 1]?.icon;
                    sideSpell2?.gameObject.SetActive(true);
                }
            }
            else if (spellManager.SpellsBar.Count == 7 && activeSpell == 0 && sideSpell2)
            {
                sideSpell2.sprite = spellManager.SpellsBar[7]?.icon;
                sideSpell2.gameObject.SetActive(true);
            }

            if (spellManager.SpellsBar.Count > activeSpell + 1 && activeSpell != 7 && sideSpell1)
            {
                if (spellManager.SpellsBar[activeSpell + 1] != null && spellManager.SpellsBar.Count > activeSpell + 1)
                {
                    sideSpell1.sprite = spellManager.SpellsBar[activeSpell + 1]?.icon;
                    sideSpell1.gameObject.SetActive(true);
                }
            }
            else if (spellManager.SpellsBar.Count > 0 && activeSpell == 7 && sideSpell1)
            {
                sideSpell1.sprite = spellManager.SpellsBar[0]?.icon;
                sideSpell1.gameObject.SetActive(true);
            }
            #endregion

            #region Abilities UI
            if (spellManager.AbilitiesBar.Count > activeAbility && mainAbility)
                if (spellManager.AbilitiesBar[activeAbility] != null)
                {
                    mainAbility.sprite = spellManager.AbilitiesBar[activeAbility]?.icon;
                    mainAbility.gameObject.SetActive(true);
                }

            if (spellManager.AbilitiesBar.Count > activeAbility - 1 && activeAbility != 0  && sideAbility2)
            {
                if (spellManager.AbilitiesBar[activeAbility - 1] != null && spellManager.AbilitiesBar.Count > activeAbility - 1)
                {
                    sideAbility2.sprite = spellManager.AbilitiesBar[activeAbility - 1]?.icon;
                    sideAbility2.gameObject.SetActive(true);
                }
            }
            else if (spellManager.AbilitiesBar.Count == 7 && activeAbility == 0 && sideAbility2)
            {
                sideAbility2.sprite = spellManager.AbilitiesBar[7]?.icon;
                sideAbility2.gameObject.SetActive(true);
            }

            if (spellManager.AbilitiesBar.Count > activeAbility + 1 && activeAbility != 7 && sideAbility1)
            {
                if (spellManager.AbilitiesBar[activeAbility + 1] != null && spellManager.AbilitiesBar.Count > activeAbility + 1)
                {
                    sideAbility1.sprite = spellManager.AbilitiesBar[activeAbility + 1]?.icon;
                    sideAbility1.gameObject.SetActive(true);
                }
            }
            else if (spellManager.AbilitiesBar.Count > 0 && activeAbility == 7 && sideAbility1)
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
            Spell spell = spellManager.SpellsBar[activeSpell];

            if (spell && Stats.Instance.mana.CurrentValue >= spell.cost)
            {
                //Vector2 offset = new Vector2(OffsetX, OffsetY);
                if (spell.useOffset)
                {
                    if (Side)
                    {
                        newPosition = (Vector2)castingPoint.position + spell.offset;
                    }
                    else
                    {
                        newPosition = (Vector2)castingPoint.position + new Vector2(-spell.offset.x, spell.offset.y);
                    }
                    //new ObjectPool(spellsBar[activeSpell].spellEffect, newPosition, castingPoint.rotation, 5, 10);
                    Instantiate(spell.spellEffect, newPosition, castingPoint.rotation);
                }
                else
                {
                    Instantiate(spell.spellEffect, castingPoint.position, castingPoint.rotation);
                    //new ObjectPool(spellsBar[activeSpell].spellEffect, castingPoint.position, castingPoint.rotation, 5, 10);
                }

                Stats.Instance.mana.CurrentValue -= spell.cost;

                spellCooldown = true;
                lastCastSpell = spell;
                Invoke("SpellCooldown", spell.cooldown);
                spellCooldownImg.gameObject.SetActive(true);
                spellCooldownImg.fillAmount = 1;
                PlaySpellSFX(spell);
            }
        }
        void Ability()
        {
            Spell ability = spellManager.AbilitiesBar[activeAbility];

            if (ability)
            {
                /*if (ability.name == "grappling hook")
                {
                    AbilityCooldownTracker = true;
                    Invoke("AbilityCooldown", ability.cooldown);
                    abilityCooldownImg.gameObject.SetActive(true);
                    abilityCooldownImg.fillAmount = 1;
                }*/
                if (Stats.Instance.stam.CurrentValue >= ability.cost)
                {
                    //Vector2 offset = new Vector2(OffsetX2, OffsetY2);

                    Instantiate(ability.spellEffect, castingPoint.position, castingPoint.rotation);

                    Stats.Instance.stam.CurrentValue -= ability.cost;
                    Stats.Instance.stam.StopRegen(playerData.stamRecoveryTime);

                    AbilityCooldownTracker = true;
                    lastCastAbility = ability;
                    Invoke("AbilityCooldown", ability.cooldown);
                    abilityCooldownImg.gameObject.SetActive(true);
                    abilityCooldownImg.fillAmount = 1;
                }
                PlaySpellSFX(ability);
            }
        }

        //private void SpellStartHandler() => CoreClass.GameManager.instance.Attacking1 = true;

        //private void SpellEndHandler() CoreClass.GameManager.instance.Attacking1 = false;

        private void SpellCooldown()
        {
            spellCooldown = false;
        }
        private void AbilityCooldown()
        {
            AbilityCooldownTracker = false;
            abilityCooldownImg.gameObject.SetActive(false);
        }
        
        public bool IsSpellCastable() => spellManager.SpellsBar.Count > activeSpell & playerInputHandler.SpellInput && Stats.Instance.mana.CurrentValue >= spellManager.SpellsBar[activeSpell].cost && spellCooldown == false;
        public bool IsAbilityCastable() => spellManager.AbilitiesBar.Count > activeAbility & playerInputHandler.AbilityInput && Stats.Instance.stam.CurrentValue >= spellManager.AbilitiesBar[activeAbility].cost && AbilityCooldownTracker == false;
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

        #region Audio Methods
        private void PlaySpellSFX(Spell spell)
        {
            AudioManager audioM = AudioManager.instance;
            switch (spell.name)
            {
                case "Firebolt":
                    audioM.PlayFireballLaunchSound(1.1f, 1.5f);
                    break;
                case "Frostbolt":
                    audioM.PlayWhooshSound(1.0f, 1.4f);
                    break;
                case "Lightning":
                    audioM.PlayLightningSound(0.8f, 1.2f);
                    break;
                case "Thundering Bolt":
                    audioM.PlayWhooshSound(1f, 1.4f);
                    break;
                case "Shuriken":
                    audioM.PlayWhooshSound(0.8f, 1.2f);
                    break;
            }
        }
        #endregion
    }
}
