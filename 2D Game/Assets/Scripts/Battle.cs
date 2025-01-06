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

                if (CoreClass.GameManager.Instance.Battles.Contains(BattleId))
                {
                    EndBattle();
                }
            }

            if (bossBattle)
            {
                Enemy boss = GetComponentInChildren<Enemy>();

                if (CoreClass.GameManager.Instance.BossesKilled.Contains(boss.BossId))
                {
                    boss.gameObject.SetActive(false);
                    EndBattle();
                }
                else boss.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (finalBattle && finalBattle.End && !End)
            {
                EndBattle();
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

            EndBattle();
        }

        private void EndBattle()
        {
            End = true;
            if (objectToEnable) objectToEnable.SetActive(true);
            if (doorToUnlock) doorToUnlock.Open(true);

            gameObject.SetActive(false);
            SaveSystem.LoadPlayer();

            if (!finalBattle)
            {
                CoreClass.GameManager.Instance.Battles.Add(BattleId);
            }
        }
    }
}
