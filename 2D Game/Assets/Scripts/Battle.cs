using Krisnat.Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Krisnat
{
    public class Battle : MonoBehaviour
    {
        [SerializeField] private Battle finalBattle;
        [SerializeField] private string battleId;
        [SerializeField] private bool bossBattle;
        [SerializeField] private Battle previousBattle;
        [SerializeField] private GameObject objectToEnable;
        [SerializeField] private Door doorToUnlock;
        private List<Enemy> encounter;

        public bool End { get; private set; }
        public string BattleId { get => battleId; private set => battleId = value; }

        private void Start()
        {
            encounter = GetComponentsInChildren<Enemy>(true).ToList();

            if (!finalBattle)
            {
                if (string.IsNullOrEmpty(BattleId)) BattleId = gameObject.name;

                PlayerSaveData data = SaveSystem.LoadPlayer();
                Debug.Log(data.bonfiresLitId[5]);
                if (data != null && data.bonfiresLitId != null && data.bonfiresLitId.Contains(BattleId))
                {
                    End = true;
                }
            }

            if (bossBattle)
            {
                Enemy boss = GetComponentInChildren<Enemy>();

                if (CoreClass.GameManager.Instance.BossesKilled.Contains(boss.BossId))
                {
                    boss.gameObject.SetActive(false);
                    End = true;
                }
                else boss.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (finalBattle && finalBattle.End)
            {
                End = true;
            }

            if (End)
            {
                if (objectToEnable) objectToEnable.SetActive(true);
                if (doorToUnlock) doorToUnlock.Open(true);

                gameObject.SetActive(false);
                SaveSystem.LoadPlayer();

                if (!finalBattle) CoreClass.GameManager.Instance.Battles.Add(BattleId);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.GetComponent<Player>())
            {
                if (previousBattle && !previousBattle.End) return;

                IfBattleOver();
            }
        }

        public void IfBattleOver()
        {
            foreach (Enemy enemy in encounter)
            {
                if (!enemy.Dead)
                {
                    enemy.gameObject.SetActive(true);
                }
            }
            if (!encounter.All(obj => obj.Dead)) return;

            End = true;
        }
    }
}
