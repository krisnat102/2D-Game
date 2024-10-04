using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private AudioSource buySound;
    [SerializeField] private AudioSource coinPickupSound;
    [SerializeField] private AudioSource swordSoundEffect;
    [SerializeField] private AudioSource bowSoundEffect;

    private bool muteTracker;
    #endregion

    #region Method Variables
    public AudioSource BuySound { get => buySound; private set => buySound = value; }
    public AudioSource CoinPickupSound { get => coinPickupSound; private set => coinPickupSound = value; }
    public AudioSource SwordSoundEffect { get => swordSoundEffect; private set => swordSoundEffect = value; }
    public AudioSource BowSoundEffect { get => bowSoundEffect; private set => bowSoundEffect = value; }
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
            musicMixer.SetFloat("volume", musicSave);
            sfxMixer.SetFloat("volume", sfxSave);
        }

        SaveAudioSettings();
    }
    public void MusicAudio(float volume)
    {
        if (!muteTracker)
        {
            musicMixer.SetFloat("volume", Mathf.Log10(volume) * 20);

            musicSave = volume;

            SaveAudioSettings();
        }
        else
        {
            musicSave = volume;
        } 
    }
    public void SFXAudio(float volume)
    {
        if (!muteTracker)
        {
            sfxMixer.SetFloat("volume", Mathf.Log10(volume) * 20);

            sfxSave = volume;

            SaveAudioSettings();
        }
        else
        {
            sfxSave = volume;
        }
    }
        #endregion
    }