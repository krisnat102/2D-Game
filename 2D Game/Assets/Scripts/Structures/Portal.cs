using UnityEngine;

namespace Krisnat
{
    public class Portal : MonoBehaviour, IStructurable
    {
        [SerializeField] private string sceneToLoad;
        [SerializeField] private Vector3 distanceTravel;
        public void OnTriggerStay2D(Collider2D collision)
        {
            if (PlayerInputHandler.Instance.UseInput)
            {
                if(sceneToLoad == "" && distanceTravel != Vector3.zero)
                {
                    collision.transform.position += distanceTravel;
                    return;
                }
                //if(sceneToLoad == "MainMenu") collision.GetComponent<Player>().gameObject.SetActive(false);
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);

                PlayerInputHandler.Instance.UseUseInput();
            }
        }
    }
}
