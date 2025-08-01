using Krisnat.Assets.Scripts;
using System;
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

                if (CoreClass.GameManager.instance.Battles.Contains(BattleId))
                {
                    EndBattle(false);
                }
            }

            if (bossBattle)
            {
                Enemy boss = GetComponentInChildren<Enemy>();

                if (CoreClass.GameManager.instance.BossesKilled.Contains(boss.BossId))
                {
                    boss.gameObject.SetActive(false);
                    EndBattle(false);
                }
                else boss.gameObject.SetActive(true);
            }
        }


        private void Update()
        {
            if (finalBattle && finalBattle.End && !End)
            {
                EndBattle(false);
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
                    var alertness = GetComponentsInChildren<EnemyAttackAI>(true).ToList();
                    foreach (var alert in alertness)
                    {
                        alert.Alerted = true;
                    }
                    enemy.gameObject.SetActive(true);
                }
            }
            if (!encounter.All(obj => obj.Dead)) return;

            EndBattle(true);
        }

        private void EndBattle(bool playDoorAudio)
        {
            End = true;
            if (objectToEnable) objectToEnable.SetActive(true);
            if (doorToUnlock) doorToUnlock.Open(playDoorAudio);

            gameObject.SetActive(false);
            SaveSystem.LoadPlayer();

            if (!finalBattle)
            {
                CoreClass.GameManager.instance.Battles.Add(BattleId);
            }
        }
    }
}
