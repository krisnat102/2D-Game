using Inventory;
using Krisnat;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Death : MonoBehaviour
{
    [SerializeField] private float animationLength;
    [Header("Behaviour")]
    [SerializeField] private bool destroy = true;
    [SerializeField] private bool adaptSize = true;
    [SerializeField] private bool adaptDirection = true;
    [SerializeField] private GameObject objectToSpawnBeforeDeath;
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
    private TMP_Text text;
    private AudioSource[] childAudio;
    private List<Light2D> lights;

    public bool AdaptSize { get => adaptSize; private set => adaptSize = value; }
    public bool AdaptDirection { get => adaptDirection; private set => adaptDirection = value; }

    private void OnEnable()
    {
        Invoke("DeathAnimation", animationLength);
        sprite = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        popUp = GetComponent<PopUpUI>();
        text = GetComponentInChildren<TMP_Text>();
        childAudio = GetComponentsInChildren<AudioSource>();
        lights = GetComponentsInChildren<Light2D>().ToList();

        if (childAudio.Length != 0)
        {
            childAudio[0].transform.parent = CoreClass.GameManager.instance.Audios;
        }

        foreach (var light in lights)
        {
            StartCoroutine(FadeLight(light, 0, animationLength));
        }
    }
    private void Update()
    {
        if (startFade)
        {
            transparency -= Time.deltaTime * fadeTime;

            if (sprite != null)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.b, sprite.color.g, transparency);

                if (transparency <= 0)
                {
                    Die();
                }
            }
            if(ui && image != null)
            {
                image.color = new Color(image.color.r, image.color.b, image.color.g, transparency);

                if (text) text.color = new Color(text.color.r, text.color.b, text.color.g, transparency);

                if (transparency <= 0)
                {
                    Die();
                }
            }
            if (uiCanvasGroup && canvasGroup != null)
            {
                canvasGroup.alpha = transparency;

                if (transparency <= 0)
                {
                    if(popUp != null)
                    {
                        ItemPickup.itemPopUps.Remove(popUp);
                    }

                    Die();
                }
            }
        }
    }

    public void DeathAnimation()
    {
        //gameObject.SetActive(false);
        startFade = true;

        if (fadeTime != 0)
        {
            return;
        }

        Die();
    }

    public void Die()
    {
        if (objectToSpawnBeforeDeath) objectToSpawnBeforeDeath.SetActive(true);

        if (destroy) Destroy(gameObject);
        else
        {
            if (image) image.color = new Color(image.color.r, image.color.b, image.color.g, 1);
            if (sprite) sprite.color = new Color(sprite.color.r, sprite.color.b, sprite.color.g, 1);
            if (text) text.color = new Color(text.color.r, text.color.b, text.color.g, 1);
            startFade = false;
            transparency = 0;
            gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeLight(Light2D light ,float targetIntensity, float duration)
    {
        float startIntensity = light.intensity;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            light.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / duration);
            yield return null;
        }

        light.intensity = targetIntensity; // Ensure final value is set
    }
}
