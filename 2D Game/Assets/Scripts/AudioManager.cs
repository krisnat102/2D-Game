using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public Toggle Mute;
    public Slider Music;
    public Slider SFX;

    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }  
    }

    public void MuteAudio()
    {
        int muteVolume = Mute.isOn ? 1 : 0; //if the toggle is off the value is 0 if its on its 1

        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume * muteVolume;
        }
    }

    public void MusicAudio()
    {
        float musicAudio = Music.value;

        foreach (Sound s in sounds)
        {
            if (s.name.Contains("Theme"))
            {
                s.source.volume = s.volume * musicAudio;
            }
        }
    }
    public void SFXAudio()
    {
        float sfxAudio = SFX.value;

        foreach (Sound s in sounds)
        {
            if (!s.name.Contains("Theme"))
            {
                s.source.volume = s.volume * sfxAudio;
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
