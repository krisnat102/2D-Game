using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Krisnat
{
    public class Battle : MonoBehaviour
    {
        [SerializeField] private Battle previousBattle;
        [SerializeField] private GameObject objectToEnable;
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
                }
            }
            if (!encounter.All(obj => obj.Dead)) return;

            End = true;

            if(objectToEnable) objectToEnable.SetActive(true);

            gameObject.SetActive(false);
        }
    }
}
