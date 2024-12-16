using Krisnat.Assets.Scripts;
using UnityEngine;

namespace Krisnat
{
    public class Portal : MonoBehaviour, IStructurable
    {
        [SerializeField] private bool loadNextScene;
        [SerializeField] private string sceneToLoad;
        [SerializeField] private Vector3 distanceTravel;
        private LevelLoader levelLoader;
        private void Start()
        {
            levelLoader = GetComponent<LevelLoader>();
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();

            if (PlayerInputHandler.Instance.UseInput && player)
            {
                PlayerInputHandler.Instance.UseUseInput();

                if (sceneToLoad == "" && distanceTravel != Vector3.zero)
                {
                    player.transform.position += distanceTravel;

                    return;
                }
                CoreClass.GameManager.checkpoint = CoreClass.GameManager.Instance.SpawnPoint.position;
                SaveSystem.SavePlayer(player);

                if (loadNextScene)
                {
                    if(levelLoader) levelLoader.LoadNextLevel();
                    else UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);

                    return;
                }
                
                if (levelLoader)
                {
                    levelLoader.LoadLevel(sceneToLoad);
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
                }
            }
        }
    }
}
