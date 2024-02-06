using UnityEngine;
using UnityEngine.U2D;

public class Death : MonoBehaviour
{
    [SerializeField] private float animationLength = 1f;
    [SerializeField] private bool fade;
    [SerializeField] private float fadeTime;

    private float transparency = 1f;
    private SpriteRenderer sprite;
    private bool startFade = false;

    private void Start()
    {
        Invoke("DeathAnimation", animationLength);
        sprite = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (startFade)
        {
            if (sprite != null)
            {
                transparency -= Time.deltaTime * fadeTime;
                sprite.color = new Color(sprite.color.r, sprite.color.b, sprite.color.g, transparency);
                if (transparency <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    public void DeathAnimation()
    {
        //gameObject.SetActive(false);
        startFade = true;
        if (fade)
        {
            return;
        }
        Destroy(gameObject);
    }
}
