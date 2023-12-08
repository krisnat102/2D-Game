using UnityEngine;

public class Death : MonoBehaviour
{
    [SerializeField] private float animationLength = 1f;

    public void Start()
    {
        Invoke("DeathAnimation", animationLength);
    }

    public void DeathAnimation()
    {
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

    
}
