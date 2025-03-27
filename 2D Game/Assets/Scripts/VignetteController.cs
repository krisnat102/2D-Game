using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

namespace Krisnat
{

    public class VignetteController : MonoBehaviour
    {
        public static VignetteController instance;

        public bool Active { get; private set; }

        public Volume volume;  // Assign your Volume in the Inspector
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
