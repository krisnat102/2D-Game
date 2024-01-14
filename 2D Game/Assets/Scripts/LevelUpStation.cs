using UnityEngine;

namespace Krisnat
{
    public class LevelUpStation : MonoBehaviour, IStructurable
    {
        public void OnTriggerStay2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();
            if (PlayerInputHandler.Instance.UseInput && player != null)
            {
                if (!UIManager.Instance.LevelUpInterface.activeInHierarchy)
                {
                    UIManager.Instance.LevelUpInterface.SetActive(true);
                }
                else
                {
                    UIManager.Instance.LevelUpInterface.SetActive(false);
                }
                PlayerInputHandler.Instance.UseUseInput();
            }
        }
    }
}
