using Bardent.CoreSystem;
using UnityEngine;

namespace Krisnat
{
    public class ChallengeRoom : MonoBehaviour
    {
        private float savedHp, savedMana;
        private Vector3 savedPos;
        private Stats pStats;

        public void EnterRoom(Player p)
        {
            pStats = p.GetComponentInChildren<Stats>();

            CoreClass.GameManager.instance.ActiveChallengeRoom = this;

            savedHp = pStats.health.CurrentValue;
            savedMana = pStats.mana.CurrentValue;
        }

        public void ExitRoom(bool success)
        {
            if (!pStats) return;

            if (success)
            {
                pStats.health.CurrentValue = pStats.health.MaxValue;
                pStats.mana.CurrentValue = pStats.mana.MaxValue;
            }
            else
            {
                pStats.health.CurrentValue = savedHp;
                pStats.mana.CurrentValue = savedMana;

                pStats.gameObject.transform.position = savedPos;
            }

            CoreClass.GameManager.instance.ActiveChallengeRoom = null;
        }
    }
}
