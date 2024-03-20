using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Krisnat
{
    public class Portal : MonoBehaviour, IStructurable
    {
        [SerializeField] private string sceneToLoad;
        public void OnTriggerStay2D(Collider2D collision)
        {
            if (PlayerInputHandler.Instance.UseInput)
            {
                //if(sceneToLoad == "MainMenu") collision.GetComponent<Player>().gameObject.SetActive(false);
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);

                PlayerInputHandler.Instance.UseUseInput();
            }
        }
    }
}
