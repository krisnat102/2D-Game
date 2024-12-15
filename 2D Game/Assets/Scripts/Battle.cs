using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Krisnat
{
    public class Battle : MonoBehaviour
    {
        [SerializeField] private bool bossBattle;
        [SerializeField] private Battle previousBattle;
        [SerializeField] private GameObject objectToEnable;
        [SerializeField] private Door doorToUnlock;
        private List<Enemy> encounter;

        public bool End { get; private set; }

        private void Start()
        {
            encounter = GetComponentsInChildren<Enemy>(true).ToList();

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
            if(End)
            {
                if (objectToEnable) objectToEnable.SetActive(true);
                if (doorToUnlock) doorToUnlock.Open(true);

                gameObject.SetActive(false);
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
