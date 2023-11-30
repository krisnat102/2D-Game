using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public static float hp = 100f;
    public static float maxHP;
    public static float stam = 100f;
    public static float maxStam;
    public static float mana = 100f;
    public static float maxMana;

    public static bool death = false;

    private bool immune = false;
    private bool stamRegenCooldown = false;

    private Transform transform;

    [Header("Stats")]

    [SerializeField] private float stamRegenSpeed = 0.20f;
    [SerializeField] private int level = 1;

    [Header("Weapons")]

    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject sword;

    [Header("UI")]

    [SerializeField] private Slider HpBar;
    [SerializeField] private Slider ManaBar;
    [SerializeField] private Slider StamBar;

    [Header("Other")]

    [SerializeField] private GameObject deathEffect;

    private float armor = 0f;
    private float magicRes = 0f;
    private float weight = 0f;

    private void Awake()
    {
        Instance = this;

        death = false;
    }

    void Start()
    {
        maxHP = hp;
        maxStam = stam;
        maxMana = mana;

        HpBar.maxValue = maxHP;
        ManaBar.maxValue = maxMana;
        StamBar.maxValue = maxStam;

        transform = FindObjectOfType<Transform>();
    }

    void Update()
    {
        if (GameManager.gamePaused == false)
        {
            HpBar.value = hp;
            ManaBar.value = mana;
            StamBar.value = stam;

            if (hp <= 0)
            {
                Die();
            }

            if (Input.GetButtonDown("Gun"))
            {
                gun.SetActive(true);
                sword.SetActive(false);
            }

            if (Input.GetButtonDown("Sword"))
            {
                gun.SetActive(false);
                sword.SetActive(true);
            }

            StamRegen();
        }
    }

    public void TakeDamage(float damage)
    {
        if (immune == false)
        {
            hp -= damage;

            immune = true;

            Invoke("StopImmune", 0.2f);
        }
        if (hp <= 0)
        {

            Die();
        }
    }

    private void StopImmune()
    {
        immune = false;
    }

    void Die()
    {
        GameObject Death = Instantiate(deathEffect, transform.position, transform.rotation);

        death = true;
    }

    public void Heal(int value)
    {
        if(hp < maxHP)
        {
            hp += value;
            if (hp > maxHP)
            {
                hp = maxHP;
            }
        }
    }

    public void HealMana(int value)
    {
        if (mana < maxMana)
        {
            hp += value;
            if (mana > maxMana)
            {
                mana = maxMana;
            }
        }
    }

    private void StamRegen()
    {
        if (stam < maxStam && stamRegenCooldown != true)
        {
            stamRegenCooldown = true;

            stam++;

            Invoke("StamRegenCooldown", stamRegenSpeed);
        }
    }

    private void StamRegenCooldown()
    {
        stamRegenCooldown = false;
    }

    public void RefreshStats()
    {
        if (InventoryManager.Instance.HelmetBn.GetComponent<ItemController>().GetItem() != null)
        {
            armor += InventoryManager.Instance.HelmetBn.GetComponent<ItemController>().GetItem().armor;
            magicRes += InventoryManager.Instance.HelmetBn.GetComponent<ItemController>().GetItem().magicRes;
            weight += InventoryManager.Instance.HelmetBn.GetComponent<ItemController>().GetItem().weight;
        }

        if (InventoryManager.Instance.ChestplateBn.GetComponent<ItemController>().GetItem() != null)
        {
            armor += InventoryManager.Instance.ChestplateBn.GetComponent<ItemController>().GetItem().armor;
            magicRes += InventoryManager.Instance.ChestplateBn.GetComponent<ItemController>().GetItem().magicRes;
            weight += InventoryManager.Instance.ChestplateBn.GetComponent<ItemController>().GetItem().weight;
        }

        if (InventoryManager.Instance.GlovesBn.GetComponent<ItemController>().GetItem() != null)
        {
            armor += InventoryManager.Instance.GlovesBn.GetComponent<ItemController>().GetItem().armor;
            magicRes += InventoryManager.Instance.GlovesBn.GetComponent<ItemController>().GetItem().magicRes;
            weight += InventoryManager.Instance.GlovesBn.GetComponent<ItemController>().GetItem().weight;
        }
        if (InventoryManager.Instance.BootsBn.GetComponent<ItemController>().GetItem() != null)
        {
            armor += InventoryManager.Instance.BootsBn.GetComponent<ItemController>().GetItem().armor;
            magicRes += InventoryManager.Instance.BootsBn.GetComponent<ItemController>().GetItem().magicRes;
            weight += InventoryManager.Instance.BootsBn.GetComponent<ItemController>().GetItem().weight;
        }
        Debug.Log("armor " + armor);
        Debug.Log("magicRes " + magicRes);
        Debug.Log("weight " + weight);
    }
}
