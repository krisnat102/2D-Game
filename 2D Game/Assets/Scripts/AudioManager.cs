using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private AudioMixer sfxMixer;

    //public Sound[] sounds;

    /*public static float music = 1f;
    public static float sfx = 1f;
    public static int audioMute = 1;*/

    [SerializeField] private Slider Music;
    [SerializeField] private Slider SFX;
    [SerializeField] private Toggle Mute;

    public static float musicSave = 0;
    public static float sfxSave = 0;

    private void Start()
    {
        if(musicSave != 0)
        {
            Music.value = musicSave;
        }
        if (sfxSave != 0)
        {
            SFX.value = sfxSave;
        }

        LoadAudioSettings();
    }

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

    /*void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if (s.name.Contains("Theme"))
            {
                s.source.volume = s.volume * music * audioMute;
            }
            else
            {
                s.source.volume = s.volume * sfx * audioMute;
            }

            Music.value = music;
            SFX.value = sfx;
            Mute.isOn = (audioMute == 0); //will set to true if audioMute is 0 and to false otherwise
        }
        
    }*/

    public void MuteAudio(bool mute)
    {
        Mute.isOn = mute;
        if (mute == true)
        {
            musicMixer.SetFloat("volume",-80);
            sfxMixer.SetFloat("volume", -80);
        }
        else
        {
            musicMixer.SetFloat("volume", musicSave);
            sfxMixer.SetFloat("volume", sfxSave);
        }

        SaveAudioSettings();
        //audioMute = muteVolume;

        /*foreach (Sound s in sounds)
        {
            s.source.volume = s.volume * muteVolume;
        }*/
    }

    public void MusicAudio(float volume)
    {
        //music = volume;

        musicMixer.SetFloat("volume", Mathf.Log10(volume) * 20);

        musicSave = volume;

        SaveAudioSettings();
    }
    public void SFXAudio(float volume)
    {
        //sfx = volume;

        sfxMixer.SetFloat("volume", Mathf.Log10(volume) * 20);

        sfxSave = volume;

        SaveAudioSettings();
    }

    /*private void Start()
    {
        Play("Theme");
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null) return;

        s.source.Play();
    }*/
}