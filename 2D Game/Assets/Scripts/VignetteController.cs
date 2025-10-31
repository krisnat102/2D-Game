using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using Bardent.CoreSystem;

namespace Krisnat
{

    public class VignetteController : MonoBehaviour
    {
        public static VignetteController instance;

        public bool Active { get; private set; }

        public Volume volume; 
        private Vignette vignette;
        private float activeVignetteThreshold;

        private void Awake()
        {
            instance = this;
            VolumeProfile profile = volume.sharedProfile;
            profile.TryGet(out vignette);
            activeVignetteThreshold = vignette.intensity.value;
        }

        public void ChangeVignette(float targetIntensity, float duration)
        {
            StartCoroutine(IncreaseVignette(targetIntensity, duration));

            if (targetIntensity > activeVignetteThreshold) Active = true;
            else Active = false;
        }

        public void SetVignette(float targetIntensity)
        {
            if(vignette) vignette.intensity.value = targetIntensity;
        }

        public void BlinkVignetteEffect(float targetIntensity, float duration)
        {
            StartCoroutine(BlinkVignette(targetIntensity, duration));
        }

        private IEnumerator BlinkVignette(float targetIntensity, float duration)
        {
            if (Stats.instance.LowHP)
            {
                yield return StartCoroutine(IncreaseVignette(targetIntensity * 2, duration / 2));
                yield return StartCoroutine(IncreaseVignette(0.5f, duration / 2));
            }
            else
            {
                yield return StartCoroutine(IncreaseVignette(targetIntensity, duration / 2));
                yield return StartCoroutine(IncreaseVignette(0, duration / 2));
            }
        }

        private IEnumerator IncreaseVignette(float targetIntensity, float duration)
        {
            float startIntensity = vignette.intensity.value;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / duration);
                yield return null;
            }
            vignette.intensity.value = targetIntensity;
        }
    }


}
