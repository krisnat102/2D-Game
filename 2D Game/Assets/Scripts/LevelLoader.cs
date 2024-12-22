using Krisnat.Assets.Scripts;
using System.Collections;
using UnityEngine;

namespace Krisnat
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Animator transition;
        [SerializeField] private float transitionTime = 1f;

        public void LoadNextLevel() => StartCoroutine(LoadLevelCoroutine(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1));
        public void LoadMainMenu()
        {
            StartCoroutine(DestroyPlayer(transitionTime - 0.1f));
            StartCoroutine(LoadLevelCoroutine(0));
            CoreClass.GameManager.Instance.GamePaused = false;
        }

        public void LoadLevel(int buildIndex) => StartCoroutine(LoadLevelCoroutine(buildIndex));
        public void LoadLevel(string sceneName) => StartCoroutine(LoadLevelCoroutine(sceneName));

        public void LoadLoadedLevel()
        {
            Debug.Log("start");
            StartCoroutine(LoadLevelCoroutine(MenuManager.Instance.CurrentLevel));
            Debug.Log("start1");
        }

        public IEnumerator LoadLevelCoroutine(int buildIndex)
        {
            Debug.Log(buildIndex);
            StartCoroutine(DestroyPlayer(transitionTime - 0.1f));
            transition.SetTrigger("Start");
            if (buildIndex != 0) MenuManager.Instance.CurrentLevel = buildIndex;

            if (CoreClass.GameManager.Instance && CoreClass.GameManager.Instance.Player) CoreClass.GameManager.Instance.SavePlayer();

            yield return new WaitForSeconds(transitionTime);

            UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
        }

        public IEnumerator LoadLevelCoroutine(string sceneName)
        {
            StartCoroutine(DestroyPlayer(transitionTime - 0.1f));
            transition.SetTrigger("Start");

            yield return new WaitForSeconds(transitionTime);

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            MenuManager.Instance.CurrentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        }

        IEnumerator DestroyPlayer(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if(CoreClass.GameManager.Instance && CoreClass.GameManager.Instance.PlayerGO) Destroy(CoreClass.GameManager.Instance.PlayerGO);
        }
    }
}
