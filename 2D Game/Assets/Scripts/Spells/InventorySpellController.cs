using Inventory;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spells
{
    public class InventorySpellController : MonoBehaviour
    {
        Spell spell;

        [SerializeField] private Image selectedItemIndicator;
        [SerializeField] private int ActiveSpellsMax = 8;

        private Button useButton;
        private Image spellImage;
        private TMP_Text spellName, spellPrice, spellValue, spellDescription;
        private GameObject description;

        public Image SelectedItemIndicator { get => selectedItemIndicator; private set => selectedItemIndicator = value; }

        public void RemoveSpell()
        {
            SpellManager.Instance.Remove(spell);

            Destroy(gameObject);
        }

        public void AddSpell(Spell newSpell)
        {
            spell = newSpell;
        }

        public void UseSpell()
        {
            GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            SpellController spellController = button.GetComponent<SpellController>();
            Spell spell = spellController.GetSpell();

            if (SpellManager.Instance.SpellsBar.Count < ActiveSpellsMax && !SpellManager.Instance.SpellsBar.Contains(spell) && spell.spell)
            {
                SpellManager.Instance.SpellsBar.Add(spell);
                SpellManager.Instance.ListActiveSpells();
            }

            if (SpellManager.Instance.AbilitiesBar.Count < ActiveSpellsMax && !SpellManager.Instance.AbilitiesBar.Contains(spell) && !spell.spell)
            {
                SpellManager.Instance.AbilitiesBar.Add(spell);
                SpellManager.Instance.ListActiveSpells();
            }
        }

        public void RemoveActiveSpell()
        {
            GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            SpellController spellController = button.GetComponent<SpellController>();

            Destroy(gameObject);

            SpellManager.Instance.SpellsBar.Remove(spellController.GetSpell());
            Debug.Log(spellController.GetSpell().name);
        }

        public void Description()
        {
            GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            SpellController spellController = button.GetComponent<SpellController>();

            description = SpellManager.Instance.description1;
            description.SetActive(true);

            spellImage = SpellManager.Instance.spellImage1;
            spellName = SpellManager.Instance.spellName1;
            spellDescription = SpellManager.Instance.spellDescription1;
            spellValue = SpellManager.Instance.spellValue1;
            spellPrice = SpellManager.Instance.spellPrice1;

            spellImage.sprite = spellController.GetSpell().icon;
            spellName.text = spellController.GetSpell().spellName.ToUpper();
            spellDescription.text = spellController.GetSpell().description;
            if (spellController.GetSpell().value != 0)
            {
                spellValue.text = "DMG - " + spellController.GetSpell().value.ToString();
            }
            else spellValue.text = null;
            spellPrice.text = "COST - " + spellController.GetSpell().cost.ToString();

            useButton = SpellManager.Instance.useButton1;
            SpellController useButtonSpellController = useButton.GetComponent<SpellController>();
            useButtonSpellController.SetSpell(spellController.GetSpell());
        }

        public void DisableSelectedIndicators() => SpellManager.Instance.DisableSelectedIndicators();
    }
}