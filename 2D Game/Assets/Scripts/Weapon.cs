using UnityEngine;
using UnityEngine.Audio;

public class Weapon : MonoBehaviour
{
    [SerializeField] private AudioSource reload;
    [SerializeField] private AudioSource shoot;

    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bullet;

    [SerializeField] private float fireRate = 2.0f;
    private float nextFireTime = 0.0f;
    [SerializeField] private int maxMagCapacity = 8;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private float reloadSpeed = 2f;

    private Animator gunAnimator;

    private int magCapacity = 8;
    bool reloading = false;

    public static bool canFire = true;

    void Start()
    {
        magCapacity = maxMagCapacity;

        gunAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.gamePaused == false)
        {
            if (Input.GetButton("Fire1") && canFire == true)
            {
                Shoot();
            }
            Reload();
        }
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

            shoot.Play();
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

            //FindObjectOfType<AudioManager>().Play("GunReload");

            reload.Play();
        }
    }

    void Reload2()
    {
        magCapacity = maxMagCapacity;

        reloading = false;
    }

    public int GetCurrentCapacity()
    {
        return magCapacity;
    }
    public int GetMaxCapacity()
    {
        return maxMagCapacity;
    }
}
