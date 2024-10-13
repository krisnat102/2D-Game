using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class Battle : MonoBehaviour
    {
        [SerializeField] private List<Enemy> encounter;
        [SerializeField] private Battle previousBattle;

        public bool End { get; private set; }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Player>())
            {
                if (previousBattle && !previousBattle.End) return;

                foreach(Enemy enemy in encounter)
                {
                    enemy.gameObject.SetActive(true);
                }
            }
        }

        public void IfBattleOver()
        {
            foreach (Enemy enemy in encounter)
            {

            }

                End = true;
        }
    }
}
