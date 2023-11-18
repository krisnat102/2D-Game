using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool gamePaused = false;

    public static int money = 0;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject weapon1;
    [SerializeField] private GameObject weapon2;

    [SerializeField] private GameObject deathScreen;

    private void Update()
    {
        if(PlayerStats.death == true)
        {
            Destroy(player);
            Destroy(weapon1);
            Destroy(weapon2);

            deathScreen.SetActive(true);        
        }
    }

    public void TryAgain()
    {
  
        Application.LoadLevel(Application.loadedLevel);

        PlayerStats.death = false;

        PlayerStats.hp = PlayerStats.maxHP;
        PlayerStats.mana = PlayerStats.maxMana;
        PlayerStats.stam = PlayerStats.maxStam;
    }
}
