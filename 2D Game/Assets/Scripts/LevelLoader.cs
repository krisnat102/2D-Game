using System.Collections;
using UnityEngine;

namespace Krisnat
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Animator transition;
        [SerializeField] private float transitionTime = 1f;

        public void LoadNextLevel() => StartCoroutine(LoadLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1));
        public void LoadMainMenu()
        {
            StartCoroutine(DestroyPlayer(transitionTime - 0.1f));
            StartCoroutine(LoadLevel(0));
            CoreClass.GameManager.Instance.GamePaused = false;
        }

        IEnumerator LoadLevel(int levelIndex)
        {
            transition.SetTrigger("Start");

            yield return new WaitForSeconds(transitionTime);

            UnityEngine.SceneManagement.SceneManager.LoadScene(levelIndex);
        }

        IEnumerator DestroyPlayer(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Destroy(CoreClass.GameManager.Instance.Player);
        }
    }
}
