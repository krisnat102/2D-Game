using System.Collections;
using UnityEngine;

namespace Krisnat
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Animator transition;
        [SerializeField] private float transitiontime = 1f;

        public void LoadNextLevel() => StartCoroutine(LoadLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1));

        IEnumerator LoadLevel(int levelIndex)
        {
            transition.SetTrigger("Start");

            yield return new WaitForSeconds(transitiontime);

            UnityEngine.SceneManagement.SceneManager.LoadScene(levelIndex);
        }
    }
}
