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

    bool immune = false;

    private bool stamRegenCooldown = false;
    [SerializeField] private float stamRegenSpeed = 0.20f;

    [SerializeField] private GameObject deathEffect;
    public static bool death = false;

    [SerializeField] private Transform transform;

    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject sword;

    [SerializeField] private Slider HpBar;
    [SerializeField] private Slider ManaBar;
    [SerializeField] private Slider StamBar;

    public int level = 1;

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
        GameObject Death = GameObject.Instantiate(deathEffect, transform.position, transform.rotation);

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
}
