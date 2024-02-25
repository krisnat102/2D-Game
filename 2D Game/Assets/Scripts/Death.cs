using Inventory;
using Krisnat;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Death : MonoBehaviour
{
    [SerializeField] private float animationLength = 1f;
    [Header("Behaviour")]
    [SerializeField] private bool destroy = true;
    [SerializeField] private GameObject objectToSpawnBeforeDeath;
    [SerializeField] private bool fade;
    [SerializeField] private float fadeTime;
    [Header("Types")]
    [SerializeField] private bool ui;
    [SerializeField] private bool uiCanvasGroup;

    private float transparency = 1f;
    private SpriteRenderer sprite;
    private Image image;
    private CanvasGroup canvasGroup;
    private PopUpUI popUp;
    private bool startFade = false;

    private void OnEnable()
    {
        Invoke("DeathAnimation", animationLength);
        sprite = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        popUp = GetComponent<PopUpUI>();    
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
            if(ui && image != null)
            {
                transparency -= Time.deltaTime * fadeTime;
                image.color = new Color(image.color.r, image.color.b, image.color.g, transparency);
                if (transparency <= 0)
                {
                    Destroy(gameObject);
                }
            }
            if (uiCanvasGroup && canvasGroup != null)
            {
                transparency -= Time.deltaTime * fadeTime;
                canvasGroup.alpha = transparency;
                if (transparency <= 0)
                {
                    if(popUp != null)
                    {
                        ItemPickup.itemPopUps.Remove(popUp);
                    }
                    if (objectToSpawnBeforeDeath) objectToSpawnBeforeDeath.SetActive(true);
                    if (destroy) Destroy(gameObject);
                    else gameObject.SetActive(false);
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
        if (objectToSpawnBeforeDeath) objectToSpawnBeforeDeath.SetActive(true);
        if (destroy) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}
