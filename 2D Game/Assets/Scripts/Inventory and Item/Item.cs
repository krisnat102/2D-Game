using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Item/Create New ITem")]
public class Item : ScriptableObject
{
    public int id;
    public string ItemName;
    public int value;
    public int cost;
    public bool usable;
    public Sprite icon;
    public ItemType itemType;
    public string itemDescription;

    public enum ItemType
    {
        Potion,
        Food,
        Gem
    }
}
