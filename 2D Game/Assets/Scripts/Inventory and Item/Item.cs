using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New ITem")]
    public class Item : ScriptableObject
    {
        #region Public Variables
        public string itemName;
        public int value;
        public int cost;
        public bool usable;
        public Sprite icon;
        public ItemClass itemClass;
        [TextArea]
        public string itemDescription;

        public enum ItemClass
        {
            Consumable,
            Material,
            Equipment,
            Weapon,
            Quest,
            Misc
        }
        #endregion

        #region Private Variables
        private bool equipped = false;
        private int itemCount = 1;
        #endregion

        #region Method Variables
        public bool Equipped { get => equipped; private set => equipped = value; }
        public int ItemCount { get => itemCount; private set => itemCount = value; }
        #endregion

        #region Variable Dependencies
        [HideInInspector]
        public bool equipment;
        [HideInInspector]
        public float armor;
        [HideInInspector]
        public float weight;
        [HideInInspector]
        public float magicRes;
        [HideInInspector]
        public EquipmentType equipmentType;

        [HideInInspector]
        public bool consumable;
        [HideInInspector]
        public ConsumableType consumableType;

        public enum EquipmentType
        {
            None,
            Helmet,
            Chestplate,
            Gloves,
            Leggings
        }
        public enum ConsumableType
        {
            None,
            Heal,
            ManaHeal
        }
        #endregion

        #region Setters
        public void SetEquipped(bool equip)
        {
            Equipped = equip;
        }
        public void SetItemCount(int count)
        {
            ItemCount = count;
        }
        public void IncreaseItemCount()
        {
            ItemCount++;
        }
        public void DecreaseItemCount()
        {
            ItemCount--;
        }
        #endregion
    }
    #region Variable Dependency Class
#if UNITY_EDITOR
    [CustomEditor(typeof(Item))]
    public class Item_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // for other non-HideInInspector fields

            Item script = (Item)target;

            // draw checkbox for the bool
            script.equipment = EditorGUILayout.Toggle("Equipment", script.equipment);
            if (script.equipment) // if bool is true, show other fields
            {
                script.armor = EditorGUILayout.FloatField("Armor", script.armor);
                script.weight = EditorGUILayout.FloatField("Weight", script.weight);
                script.magicRes = EditorGUILayout.FloatField("Magic Resistance", script.magicRes);
                script.equipmentType = (Item.EquipmentType)EditorGUILayout.EnumPopup("Type", script.equipmentType);
            }

            script.consumable = EditorGUILayout.Toggle("Consumable", script.consumable);
            if (script.consumable)
            {
                script.consumableType = (Item.ConsumableType)EditorGUILayout.EnumPopup("Type", script.consumableType);
            }
        }
    }
}
#endif
#endregion