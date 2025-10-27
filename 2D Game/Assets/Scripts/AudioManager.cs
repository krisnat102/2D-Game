using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System.IO;
using Krisnat;
// ReSharper disable CheckNamespace

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    #region Private Variables
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private AudioMixer sfxMixer;
    [SerializeField] private AudioMixer environmentSfxMixer;
    [FormerlySerializedAs("Music")] [SerializeField] private Slider music;
    [FormerlySerializedAs("SFX")] [SerializeField] private Slider sfx;
    [SerializeField] private Slider environmentSfx;
    [FormerlySerializedAs("Mute")] [SerializeField] private Toggle mute;

    [Header("Sounds")]
    [SerializeField] private AudioSource generalSound;
    [FormerlySerializedAs("music")] [SerializeField] private AudioSource musicSound;
    [SerializeField] private AudioSource buttonSound, menuSound;
    [SerializeField] private AudioSource buySound, coinPickupSound, tradeRefusedSound;
    [SerializeField] private AudioSource heartbeatSound;
    [SerializeField] private AudioSource weaponSound, whooshSoundEffect, dodgeRollSoundEffect, jumpSoundEffect;
    [SerializeField] private AudioSource fireballLaunchEffect, lightningEffect;
    [SerializeField] private AudioSource eatSoundEffect, potionDrinkSoundEffect;
    [SerializeField] private AudioSource[] equipmentSoundEffect;

    private bool muteTracker, muteSave;
    private float musicSave, sfxSave, environmentSfxSave;
    
    private string SettingsFile => Path.Combine(Application.persistentDataPath, "settings.json");

    private SettingsData settings = new SettingsData();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadAudioSettings();
    }
    #endregion

    #region Settings

    private void SaveAudioSettings()
    {
        settings.musicVolume = musicSave;
        settings.sfxVolume = sfxSave;
        settings.environmentSfx = environmentSfxSave;
        settings.mute = mute.isOn;

        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(SettingsFile, json);
    }

    private void LoadAudioSettings()
    {
        if (File.Exists(SettingsFile))
        {
            string json = File.ReadAllText(SettingsFile);
            settings = JsonUtility.FromJson<SettingsData>(json);
                
            musicSave = settings.musicVolume;
            sfxSave = settings.sfxVolume;
            environmentSfxSave = settings.environmentSfx;
            muteSave = settings.mute;
            //Debug.Log($"{musicSave} {sfxSave} {environmentSfxSave}");
        }
        else
        {
            //Creates a new file if no file exists
            settings = new SettingsData();
        }
    
        //Apply settings to ui
        music.value = Mathf.InverseLerp(0.0001f, 1f, musicSave);
        sfx.value = Mathf.InverseLerp(0.0001f, 1f, sfxSave);
        environmentSfx.value = Mathf.InverseLerp(0.0001f, 1f, environmentSfxSave);
        
        musicMixer.SetFloat("volume", Mathf.Log10(musicSave) * 20);
        sfxMixer.SetFloat("volume", Mathf.Log10(sfxSave) * 20);
        environmentSfxMixer.SetFloat("volume", Mathf.Log10(environmentSfxSave) * 20);
        
        if (muteSave) MuteAudio(true);
    }

    #endregion

    #region Audio
    public void PauseMusic() => musicSound.Pause();
    public void UnPauseMusic() => musicSound.UnPause();

    public void MuteAudio(bool muteValue)
    {
        mute.isOn = muteValue;
        muteTracker = muteValue;

        if (muteValue)
        {
            musicMixer.SetFloat("volume", -80);
            sfxMixer.SetFloat("volume", -80);
            environmentSfxMixer.SetFloat("volume", -80);
        }
        else
        {
            musicMixer.SetFloat("volume", Mathf.Log10(musicSave) * 20);
            sfxMixer.SetFloat("volume", Mathf.Log10(sfxSave) * 20);
            environmentSfxMixer.SetFloat("volume", Mathf.Log10(environmentSfxSave) * 20);
        }

        SaveAudioSettings();
    }
    public void MusicAudio(float volume)
    {
        musicSave = volume;

        if (muteTracker) return;
        
        musicMixer.SetFloat("volume", Mathf.Log10(volume) * 20);

        SaveAudioSettings();
    }
    public void SfxAudio(float volume)
    {
        sfxSave = volume;

        if (muteTracker) return;
        
        sfxMixer.SetFloat("volume", Mathf.Log10(volume) * 20);

        SaveAudioSettings();
    }
    public void EnvironmentSfxAudio(float volume)
    {
        environmentSfxSave = volume;

        if (muteTracker) return;
        
        environmentSfxMixer.SetFloat("volume", Mathf.Log10(volume) * 20);

        SaveAudioSettings();
    }
    #endregion

    #region Audio Players
    public void PlayHeartbeatSound(float lowerPitch, float higherPitch)
    {
        heartbeatSound.pitch = Random.Range(lowerPitch, higherPitch);
        heartbeatSound.Play();
    }
    public void PlayWeaponSound(AudioClip sfxValue, float lowerPitch, float higherPitch)
    {
        weaponSound.clip = sfxValue;
        weaponSound.pitch = Random.Range(lowerPitch, higherPitch);
        weaponSound.Play();
    }
    public void PlayButtonSound(float lowerPitch, float higherPitch)
    {
        buttonSound.pitch = Random.Range(lowerPitch, higherPitch);
        buttonSound.Play();
    }
    public void PlayMenuSound(float lowerPitch, float higherPitch)
    {
        menuSound.pitch = Random.Range(lowerPitch, higherPitch);
        menuSound.Play();
    }
    public void PlayBuySound(float lowerPitch, float higherPitch)
    {
        buySound.pitch = Random.Range(lowerPitch, higherPitch);
        buySound.Play();
    }

    public void PlayCoinPickupSound(float lowerPitch, float higherPitch)
    {
        coinPickupSound.pitch = Random.Range(lowerPitch, higherPitch);
        coinPickupSound.Play();
    }

    public void PlayTradeRefusedSound(float lowerPitch, float higherPitch)
    {
        tradeRefusedSound.pitch = Random.Range(lowerPitch, higherPitch);
        tradeRefusedSound.Play();
    }

    public void PlayWhooshSound(float lowerPitch, float higherPitch)
    {
        whooshSoundEffect.pitch = Random.Range(lowerPitch, higherPitch);
        whooshSoundEffect.Play();
    }

    public void PlayJumpSound(float lowerPitch, float higherPitch)
    {
        jumpSoundEffect.pitch = Random.Range(lowerPitch, higherPitch);
        jumpSoundEffect.Play();
    }

    public void PlayDodgeRollSound(float lowerPitch, float higherPitch)
    {
        dodgeRollSoundEffect.pitch = Random.Range(lowerPitch, higherPitch);
        dodgeRollSoundEffect.Play();
    }

    public void PlayFireballLaunchSound(float lowerPitch, float higherPitch)
    {
        fireballLaunchEffect.pitch = Random.Range(lowerPitch, higherPitch);
        fireballLaunchEffect.Play();
    }

    public void PlayLightningSound(float lowerPitch, float higherPitch)
    {
        lightningEffect.pitch = Random.Range(lowerPitch, higherPitch);
        lightningEffect.Play(); 
    }

    public void PlayEatSound(float lowerPitch, float higherPitch)
    {
        eatSoundEffect.pitch = Random.Range(lowerPitch, higherPitch);
        eatSoundEffect.Play();
    }

    public void PlayPotionDrinkSound(float lowerPitch, float higherPitch)
    {
        potionDrinkSoundEffect.pitch = Random.Range(lowerPitch, higherPitch);
        potionDrinkSoundEffect.Play();
    }

    public void PlayEquipmentSound(float lowerPitch, float higherPitch)
    {
        int randomSfx = Mathf.RoundToInt(Random.Range(0, equipmentSoundEffect.Length));
        equipmentSoundEffect[randomSfx].pitch = Random.Range(lowerPitch, higherPitch);
        equipmentSoundEffect[randomSfx].Play();
    }

    public void StopHeartbeatSound() => heartbeatSound.Stop();
    #endregion
}