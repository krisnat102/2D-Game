using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public static float hp = 100f;
    public static float maxHP;

    public static float stam = 100f;
    public static float maxStam;
    private bool stamRegenCooldown = false;
    [SerializeField] private float stamRegenSpeed = 0.20f;

    public GameObject deathEffect;

    new Transform transform;

    public GameObject gun;
    public GameObject sword;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        maxHP = hp;
        maxStam = stam;
    }

    void Update()
    {
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

    void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(this.gameObject);
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
