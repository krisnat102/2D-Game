using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Krisnat
{
    public class Battle : MonoBehaviour
    {
        [SerializeField] private Battle previousBattle;
        private List<Enemy> encounter;

        public bool End { get; private set; }

        private void Start()
        {
            encounter = GetComponentsInChildren<Enemy>(true).ToList();
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
                    return;
                }
            }

            End = true;
        }
    }
}
