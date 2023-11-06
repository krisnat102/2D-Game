using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;
    public AudioMixer mainMixer;

    //public Sound[] sounds;

    /*public static float music = 1f;
    public static float sfx = 1f;
    public static int audioMute = 1;*/

    public Slider Music;
    public Slider SFX;
    public Toggle Mute;

    float musicValue;
    float sfxValue;

    private void Start()
    {
        if (musicMixer.GetFloat("volume", out musicValue))
        {
            Music.value = (musicValue + 80)/100;

            Debug.Log(musicValue);
        }
        if (sfxMixer.GetFloat("volume", out sfxValue))
        {
            SFX.value = (sfxValue + 80) / 100;

            Debug.Log(sfxValue);
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
            Music.value = PlayerPrefs.GetFloat("MusicVolume");
            musicMixer.SetFloat("volume", Mathf.Log10(Music.value) * 20);
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SFX.value = PlayerPrefs.GetFloat("SFXVolume");
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
        int muteVolume = mute.Equals(true) ? 0 : 1; //if the toggle is off the value is 1 if its on its 0
        Mute.isOn = mute;

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

        SaveAudioSettings();
    }
    public void SFXAudio(float volume)
    {
        //sfx = volume;

        sfxMixer.SetFloat("volume", Mathf.Log10(volume) * 20);

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
