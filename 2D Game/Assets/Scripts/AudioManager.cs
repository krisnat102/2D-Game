using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static float music = 1f;
    public static float sfx = 1f;
    public static int audioMute = 1;

    public Slider Music;
    public Slider SFX;
    public Toggle Mute;

    void Awake()
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
        
    }

    public void MuteAudio(bool mute)
    {
        int muteVolume = mute.Equals(true) ? 0 : 1; //if the toggle is off the value is 1 if its on its 0
        audioMute = muteVolume;

        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume * muteVolume;
        }
    }

    public void MusicAudio(float volume)
    {
        music = volume;

        foreach (Sound s in sounds)
        {
            if (s.name.Contains("Theme"))
            {
                s.source.volume = s.volume * volume;
            }
        }
    }
    public void SFXAudio(float volume)
    {
        sfx = volume;

        foreach (Sound s in sounds)
        {
            if (!s.name.Contains("Theme"))
            {
                s.source.volume = s.volume * volume;
            }
        }
    }



    private void Start()
    {
        Play("Theme");
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null) return;

        s.source.Play();
    }
}
