using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

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

    public void MuteAudio(bool mute)
    {
        int muteVolume = mute.Equals(true) ? 1 : 0; //if the toggle is off the value is 0 if its on its 1

        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume * muteVolume;
        }
    }

    public void MusicAudio(float volume)
    {
        float musicAudio = volume;

        foreach (Sound s in sounds)
        {
            if (s.name.Contains("Theme"))
            {
                s.source.volume = s.volume * musicAudio;
            }
        }
    }
    public void SFXAudio(float volume)
    {
        float sfxAudio = volume;

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
