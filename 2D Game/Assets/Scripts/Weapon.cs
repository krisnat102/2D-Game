using UnityEngine;

public class Weapon : MonoBehaviour
{

    public Transform firePoint;
    public GameObject bullet;

    [SerializeField] private float fireRate = 2.0f;
    private float nextFireTime = 0.0f;
    [SerializeField] private int magCapacity = 8;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private float reloadSpeed = 2f;

    private Animator gunAnimator;

    public static int currentMagCapacity;
    public static int maxMagCapacity;
    bool reloading = false;

    public static bool canFire = true;

    void FixedUpdate()
    {
        currentMagCapacity = magCapacity;
    }

    void Start()
    {
        maxMagCapacity = magCapacity;

        gunAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") && canFire == true)
        {
            Shoot();
        }
        Reload();
    }
    void Shoot()
    {
        if(magCapacity > 0 && reloading == false) 
        { 
            if (Time.time >= nextFireTime)
            {

            Instantiate(bullet, firePoint.position, firePoint.rotation);

            nextFireTime = Time.time + 1.0f / fireRate;

            magCapacity -= bulletsPerShot;

            gunAnimator.SetTrigger("ShootTrigger");

            FindObjectOfType<AudioManager>().Play("GunFire");
            }
        }
    }
    
    void Reload()
    {
        if (maxMagCapacity != magCapacity && Input.GetButtonDown("Reload"))
        {
            reloading = true;

            Invoke("Reload2", reloadSpeed);

            gunAnimator.SetTrigger("ReloadTrigger");

            FindObjectOfType<AudioManager>().Play("GunReload");
        }
    }

    void Reload2()
    {
        magCapacity = maxMagCapacity;

        reloading = false;
    }
}
