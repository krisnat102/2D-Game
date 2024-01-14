using Bardent.CoreSystem;
using Inventory;
using TMPro;
using UnityEngine;

namespace Krisnat
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Purse")]
        [SerializeField] private GameObject purse;

        [Header("CharacterTab")]
        [SerializeField] private GameObject characterTab;
        [SerializeField] private TMP_Text levelText, hpText, manaText, stamText, armorText, magicResText, weightText;

        [SerializeField] private GameObject levelUpInterface;

        private Vector3 oldPosition;

        public GameObject LevelUpInterface { get => levelUpInterface; set => levelUpInterface = value; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            levelText.text = "1";
            hpText.text = Stats.Instance.Health.MaxValue.ToString();
            manaText.text = Stats.Instance.Mana.MaxValue.ToString();
            stamText.text = Stats.Instance.Stam.MaxValue.ToString();
            armorText.text = InventoryManager.Instance.TotalArmor.ToString();
            magicResText.text = InventoryManager.Instance.TotalMagicRes.ToString();
            weightText.text = InventoryManager.Instance.TotalWeight.ToString() + "/50";
        }

        public void MovePurseAnimation(bool directionUpOrDown, float animationDistance, float animationDuration)
        {
            if (directionUpOrDown)
            {
                //purse.transform.LeanMoveLocal(new Vector2(purse.transform.position.x, purse.transform.position.y + animationDistance), animationDuration);
                purse.transform.LeanMove(new Vector2(purse.transform.position.x, purse.transform.position.y + animationDistance), animationDuration);
                oldPosition = purse.transform.position;
            }
            else
            {
                purse.transform.LeanMove(oldPosition, animationDuration);
            }
        }

        public void OpenCharacterTabBn()
        {
            InventoryManager.Instance.OpenCloseInventory(false);

            characterTab.SetActive(true);
        }
    }
}
