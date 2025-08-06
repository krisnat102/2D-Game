using UnityEngine;
using System.Collections;

public class LandingSFX : MonoBehaviour
{
    [SerializeField] private AudioSource landAudio;
    [SerializeField] private float lowerPitch = 0.8f;
    [SerializeField] private float higherPitch = 1.2f;
    private const float COOLDOWN = 0.4f;

    private bool isOnCooldown = false;

    public void Land()
    {
        if (isOnCooldown) return;

        landAudio.pitch = Random.Range(lowerPitch, higherPitch);
        landAudio.Play();
        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(COOLDOWN);
        isOnCooldown = false;
    }
}
