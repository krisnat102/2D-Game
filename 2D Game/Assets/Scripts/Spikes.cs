using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private float spikeDmg;

    private void OnTriggerEnter2D(Collider2D hitinfo)
    {
        if(hitinfo.tag == "Player")
        {
            PlayerStats player = hitinfo.GetComponent<PlayerStats>();

            player.TakeDamage(spikeDmg);
        }
        else if(hitinfo.tag == "Enemy")
        {
            Enemy enemy = hitinfo.GetComponent<Enemy>();

            enemy.TakeDamage(spikeDmg);
        }
    }
}
