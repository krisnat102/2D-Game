using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Krisnat
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance;

        [SerializeField] private float shakeIntensity = 1f;
        [SerializeField] private float shakeTime = 0.5f;
        private CinemachineVirtualCamera virtualCam;
        private CinemachineBasicMultiChannelPerlin cbmcp;
        private bool shake;

        public float ShakeIntensity { get => shakeIntensity; set => shakeIntensity = value; }
        public float ShakeTime { get => shakeTime; set => shakeTime = value; }

        private void Awake()
        {
            Instance = this;

            virtualCam = GetComponent<CinemachineVirtualCamera>();
            cbmcp = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Start()
        {
            StopShake();
        }

        private void Update()
        {
            if (shake)
            {
                ShakeCamera();
            }

            if (PlayerInputHandler.Instance.JumpInput)
            {
                ShakeCamera();
            }
        }

        public void ShakeCamera()
        {
            cbmcp.m_AmplitudeGain = ShakeIntensity;
            shake = true;
            Invoke(nameof(StopShake), shakeTime);
        }
        public void ShakeCamera(float duration, float intensity)
        {
            ShakeTime = duration;
            ShakeIntensity = intensity;
            ShakeCamera();
        }

        private void StopShake()
        {
            cbmcp.m_AmplitudeGain = 0f;
            shake = false;
        }
    }
}
