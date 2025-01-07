using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class FrameFreeze : MonoBehaviour
    {
        public static FrameFreeze Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void FreezeFrame(float time)
        {
            StartCoroutine(UnfreezeFrame(time));
        }

        IEnumerator UnfreezeFrame(float delay)
        {
            Time.timeScale = 0;

            yield return new WaitForSecondsRealtime(delay);

            Time.timeScale = 1;
        }
    }
}
