using Bardent.CoreSystem;
using TMPro;
using UnityEngine;

namespace Krisnat
{
    public class ChallengeRoom : MonoBehaviour
    {
        [SerializeField] private AudioSource challengeRoomMusicPlayer;
        private float savedHp, savedMana;
        private Vector3 savedPos;
        private Stats pStats;

        public void EnterRoom(Player p)
        {
            pStats = p.GetComponentInChildren<Stats>();

            if (challengeRoomMusicPlayer)
            {
                challengeRoomMusicPlayer.Play();
                AudioManager.instance.FadeInSong(challengeRoomMusicPlayer, challengeRoomMusicPlayer.volume);
            }
            AudioManager.instance.PauseMusic();

            CoreClass.GameManager.instance.ActiveChallengeRoom = this;

            savedHp = pStats.health.CurrentValue;
            savedMana = pStats.mana.CurrentValue;
        }

        public void ExitRoom(bool success)
        {
            if (challengeRoomMusicPlayer) AudioManager.instance.FadeOutSong(challengeRoomMusicPlayer);
            AudioManager.instance.UnPauseMusic();

            if (!pStats) return;

            if (success)
            {
                pStats.health.CurrentValue = pStats.health.MaxValue;
                pStats.mana.CurrentValue = pStats.mana.MaxValue;

                UIManager.instance.SmallNoteUI.SetActive(true);
                UIManager.instance.SmallNoteUI.GetComponentInChildren<TMP_Text>().text = "Challenge completed!";
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
