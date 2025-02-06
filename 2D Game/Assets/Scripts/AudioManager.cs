using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AudioManager : MonoBehaviour
{
    #region Static Variables
    public static float musicSave = 0;
    public static float sfxSave = 0;
    public static AudioManager Instance;
    #endregion

    #region Private Variables
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private AudioMixer sfxMixer;
    [SerializeField] private Slider Music;
    [SerializeField] private Slider SFX;
    [SerializeField] private Toggle Mute;

    [Header("Sounds")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private AudioSource buySound, coinPickupSound, tradeRefusedSound;
    [SerializeField] private AudioSource swordSoundEffect, bowSoundEffect, dodgeRollSoundEffect;
    [SerializeField] private AudioSource fireballLaunchEffect, lightningEffect;
    [SerializeField] private AudioSource equipmentSoundEffect, eatSoundEffect, potionDrinkSoundEffect;

    private bool muteTracker;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (musicSave != 0)
        {
            Music.value = musicSave;
        }
        if (sfxSave != 0)
        {
            SFX.value = sfxSave;
        }

        LoadAudioSettings();
    }
    #endregion

    #region Settings
    public void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", Music.value);
        PlayerPrefs.SetFloat("SFXVolume", SFX.value);
        PlayerPrefs.SetInt("Mute", Mute.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            Music.value = Mathf.InverseLerp(0.0001f, 1f, PlayerPrefs.GetFloat("MusicVolume"));
            musicMixer.SetFloat("volume", Mathf.Log10(Music.value) * 20);
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SFX.value = Mathf.InverseLerp(0.0001f, 1f, PlayerPrefs.GetFloat("SFXVolume"));
            sfxMixer.SetFloat("volume", Mathf.Log10(SFX.value) * 20);
        }

        if (PlayerPrefs.HasKey("Mute"))
        {
            Mute.isOn = PlayerPrefs.GetInt("Mute") == 1;
        }
    }
    #endregion

    #region Audio
    public void PauseMusic() => music.Pause();
    public void UnPauseMusic() => music.UnPause();

    public void MuteAudio(bool mute)
    {
        Mute.isOn = mute;
        muteTracker = mute;

        if (mute)
        {
            musicMixer.SetFloat("volume", -80);
            sfxMixer.SetFloat("volume", -80);
            Debug.Log(musicSave);
            Debug.Log(sfxSave);
        }
        else
        {
            musicMixer.SetFloat("volume", Mathf.Log10(musicSave) * 20);
            sfxMixer.SetFloat("volume", Mathf.Log10(sfxSave) * 20);
        }

        SaveAudioSettings();
    }
    public void MusicAudio(float volume)
    {
        musicSave = volume;

        if (!muteTracker)
        {
            musicMixer.SetFloat("volume", Mathf.Log10(volume) * 20);

            SaveAudioSettings();
        }
    }
    public void SFXAudio(float volume)
    {
        sfxSave = volume;

        if (!muteTracker)
        {
            sfxMixer.SetFloat("volume", Mathf.Log10(volume) * 20);

            SaveAudioSettings();
        }
    }
    #endregion

    #region Players
    public void PlayButtonSound(float lowerPitch, float higherPitch)
    {
        buttonSound.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        buttonSound.Play();
    }
    public void PlayBuySound(float lowerPitch, float higherPitch)
    {
        buySound.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        buySound.Play();
    }

    public void PlayCoinPickupSound(float lowerPitch, float higherPitch)
    {
        coinPickupSound.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        coinPickupSound.Play();
    }

    public void PlayTradeRefusedSound(float lowerPitch, float higherPitch)
    {
        tradeRefusedSound.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        tradeRefusedSound.Play();
    }

    public void PlaySwordSound(float lowerPitch, float higherPitch)
    {
        swordSoundEffect.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        swordSoundEffect.Play();
    }

    public void PlayBowSound(float lowerPitch, float higherPitch)
    {
        bowSoundEffect.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        bowSoundEffect.Play();
    }

    public void PlayDodgeRollSound(float lowerPitch, float higherPitch)
    {
        dodgeRollSoundEffect.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        dodgeRollSoundEffect.Play();
    }

    public void PlayFireballLaunchSound(float lowerPitch, float higherPitch)
    {
        fireballLaunchEffect.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        fireballLaunchEffect.Play();
    }

    public void PlayLightningSound(float lowerPitch, float higherPitch)
    {
        lightningEffect.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        lightningEffect.Play();
    }

    public void PlayEquipmentSound(float lowerPitch, float higherPitch)
    {
        equipmentSoundEffect.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        equipmentSoundEffect.Play();
    }

    public void PlayEatSound(float lowerPitch, float higherPitch)
    {
        eatSoundEffect.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        eatSoundEffect.Play();
    }

    public void PlayPotionDrinkSound(float lowerPitch, float higherPitch)
    {
        potionDrinkSoundEffect.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        potionDrinkSoundEffect.Play();
    }
    #endregion
}