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
    [SerializeField] private AudioSource generalSound;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource buttonSound, menuSound;
    [SerializeField] private AudioSource buySound, coinPickupSound, tradeRefusedSound;
    [SerializeField] private AudioSource heartbeatSound;
    [SerializeField] private AudioSource weaponSound, whooshSoundEffect, dodgeRollSoundEffect;
    [SerializeField] private AudioSource fireballLaunchEffect, lightningEffect;
    [SerializeField] private AudioSource eatSoundEffect, potionDrinkSoundEffect;
    [SerializeField] private AudioSource[] equipmentSoundEffect;

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

    #region Audio Players
    public void PlayHeartbeatSound(float lowerPitch, float higherPitch)
    {
        heartbeatSound.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        heartbeatSound.Play();
    }
    public void PlayWeaponSound(AudioClip SFX, float lowerPitch, float higherPitch)
    {
        weaponSound.clip = SFX;
        weaponSound.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        weaponSound.Play();
    }
    public void PlayButtonSound(float lowerPitch, float higherPitch)
    {
        buttonSound.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        buttonSound.Play();
    }
    public void PlayMenuSound(float lowerPitch, float higherPitch)
    {
        menuSound.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        menuSound.Play();
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

    public void PlayWhooshSound(float lowerPitch, float higherPitch)
    {
        whooshSoundEffect.pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        whooshSoundEffect.Play();
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

    public void PlayEquipmentSound(float lowerPitch, float higherPitch)
    {
        int randomSFX = Mathf.RoundToInt(UnityEngine.Random.Range(0, equipmentSoundEffect.Length));
        equipmentSoundEffect[randomSFX].pitch = UnityEngine.Random.Range(lowerPitch, higherPitch);
        equipmentSoundEffect[randomSFX].Play();
    }

    public void StopHeartbeatSound() => heartbeatSound.Stop();
    #endregion
}