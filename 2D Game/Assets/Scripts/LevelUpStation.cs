using UnityEngine;

namespace Krisnat
{
    public class LevelUpStation : MonoBehaviour, IStructurable
    {
        [SerializeField] private float openingAnimationDuration = 0.2f;
        [SerializeField] private float closingAnimationDuration = 0.2f;
        public void OnTriggerStay2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();
            var levelUpUI = UIManager.Instance.LevelUpInterface;
            var scale = levelUpUI.transform.localScale.x;
            if (PlayerInputHandler.Instance.UseInput && player != null)
            {
                if (!UIManager.Instance.LevelUpInterface.activeInHierarchy)
                {
                    levelUpUI.SetActive(true);
                    levelUpUI.transform.localScale = new Vector3(0.05f, 0.05f, levelUpUI.transform.localScale.z);
                    UIManager.Instance.OpenCloseUIAnimation(levelUpUI, scale, openingAnimationDuration, true);
                }
                else
                {
                    UIManager.Instance.OpenCloseUIAnimation(levelUpUI, 0.05f, closingAnimationDuration, false);
                }
                PlayerInputHandler.Instance.UseUseInput();
            }
        }
    }
}
