using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Krisnat
{
    public class FadeOutObject : MonoBehaviour
    {
        public static FadeOutObject instance;

        private void Awake()
        {
            instance = this;
        }

        #region Fade Out
        public void FadeOutObj(GameObject obj, float fadeSpeed, float delay = 0f, bool disableAfterFadeOut = false)
        {
            var sprite = obj.GetComponent<SpriteRenderer>();
            if (sprite)
            {
                StartCoroutine(FadeOutCoroutine(sprite, fadeSpeed, delay, disableAfterFadeOut));
                return;
            }

            var tilemap = obj.GetComponent<Tilemap>();
            if (tilemap) StartCoroutine(FadeOutCoroutine(tilemap, fadeSpeed, delay, disableAfterFadeOut));
        }

        public void FadeOutObj(SpriteRenderer obj, float fadeSpeed, float delay = 0f, bool disableAfterFadeOut = false)
        {
            StartCoroutine(FadeOutCoroutine(obj, fadeSpeed, delay, disableAfterFadeOut));
        }

        public void FadeOutTilemap(Tilemap obj, float fadeSpeed, float delay = 0f, bool disableAfterFadeOut = false)
        {
            StartCoroutine(FadeOutCoroutine(obj, fadeSpeed, delay, disableAfterFadeOut));
        }

        private IEnumerator FadeOutCoroutine(SpriteRenderer obj, float fadeSpeed, float delay, bool disableAfterFadeOut)
        {
            yield return new WaitForSeconds(delay);
            Color color = obj.color;

            while (color.a > 0f)
            {
                color.a -= fadeSpeed * Time.deltaTime;
                obj.color = color;
                yield return null;
            }

            if (disableAfterFadeOut ) obj.gameObject.SetActive(false);
        }

        private IEnumerator FadeOutCoroutine(Tilemap obj, float fadeSpeed, float delay, bool disableAfterFadeOut)
        {
            yield return new WaitForSeconds(delay);
            Color color = obj.color;

            while (color.a > 0f)
            {
                color.a -= fadeSpeed * Time.deltaTime;
                obj.color = color;
                yield return null;
            }

            obj.color = color;

            if (disableAfterFadeOut) obj.gameObject.SetActive(false);
        }
        #endregion

        #region Fade In
        public void FadeInObj(SpriteRenderer obj, float fadeSpeed, float delay = 0f)
        {
            StartCoroutine(FadeInCoroutine(obj, fadeSpeed, delay));
        }

        public void FadeInTilemap(Tilemap obj, float fadeSpeed, float delay = 0f)
        {
            StartCoroutine(FadeInCoroutine(obj, fadeSpeed, delay));
        }

        private IEnumerator FadeInCoroutine(SpriteRenderer obj, float fadeSpeed, float delay)
        {
            yield return new WaitForSeconds(delay);
            Color color = obj.color;
            color.a = 0f; // start transparent
            obj.color = color;

            while (color.a < 1f)
            {
                color.a += fadeSpeed * Time.deltaTime;
                obj.color = color;
                yield return null;
            }

            color.a = 1f; // ensure fully opaque
            obj.color = color;
        }

        private IEnumerator FadeInCoroutine(Tilemap obj, float fadeSpeed, float delay)
        {
            yield return new WaitForSeconds(delay);
            Color color = obj.color;
            color.a = 0f; // start transparent
            obj.color = color;

            while (color.a < 1f)
            {
                color.a += fadeSpeed * Time.deltaTime;
                obj.color = color;
                yield return null;
            }

            color.a = 1f;
            obj.color = color;
        }
        #endregion
    }
}
