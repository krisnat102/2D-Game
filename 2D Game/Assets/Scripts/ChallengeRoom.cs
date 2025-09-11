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

            savedHp = pStats.health.CurrentValue;
            savedMana = pStats.mana.CurrentValue;
        }

        public void ExitRoomFail()
        {
            if (!pStats) return;

            pStats.health.CurrentValue = savedHp / 2;
            pStats.mana.CurrentValue = savedMana / 2;

            pStats.gameObject.transform.position = savedPos;
        }

        public void ExitRoomSuccess()
        {
            if (!pStats) return;

            pStats.health.CurrentValue = pStats.health.MaxValue;
            pStats.mana.CurrentValue = pStats.mana.MaxValue;

            pStats.gameObject.transform.position = savedPos;
        }
    }
}
