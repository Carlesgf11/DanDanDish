using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioMixer mixer;

    public const string GENERAL_KEY = "generalVolume";
    public const string MUSIC_KEY = "musicVolume";
    public const string SFX_KEY = "sfxVolume";

    public Sound[] sounds;

    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //    Destroy(gameObject);

        LoadVolume();
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.group;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void LoadVolume() //Volumen guardado en VolumeSettings
    {
        //Cojemos el valor de cada slider
        float generalVolume = PlayerPrefs.GetFloat(GENERAL_KEY, 0.6f);
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 0.6f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 0.6f);

        //Seteamos el valor
        mixer.SetFloat(VolumeSettings.MIXER_GENERAL, Mathf.Log10(generalVolume) * 20);
        mixer.SetFloat(VolumeSettings.MIXER_MUSIC, Mathf.Log10(musicVolume) * 20);
        mixer.SetFloat(VolumeSettings.MIXER_SFX, Mathf.Log10(sfxVolume) * 20);
    }
    public void PlaySound(string name) //Esta función se puede llamar donde quieras
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
            return;
        s.source.Play();
    }
    public void StopSound(string name) //No creo que sea necesario pero por si acaso
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Stop();
    }
}
