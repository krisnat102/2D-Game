using Bardent.CoreSystem;
using UnityEngine;


namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static bool gamePaused = false;

        public static int money = 0;

        [SerializeField] private GameObject deathScreen;
        [SerializeField] private GameObject player;
        [SerializeField] private Bardent.CoreSystem.Death death;

        private void Update()
        {
            if (death.IsDead == true)
            {
                deathScreen.SetActive(true);
            }
        }

        public void TryAgain()
        {
            Application.LoadLevel(Application.loadedLevel);

            death.IsDead = false;

            Stats.Instance.Health.Increase(Stats.Instance.Health.MaxValue - Stats.Instance.Health.CurrentValue);
            Stats.Instance.Mana.Increase(Stats.Instance.Mana.MaxValue - Stats.Instance.Mana.CurrentValue);
            Stats.Instance.Stam.Increase(Stats.Instance.Stam.MaxValue - Stats.Instance.Stam.CurrentValue);

            player.SetActive(true);
        }
    }
}