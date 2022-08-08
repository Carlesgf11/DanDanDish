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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        LoadVolume();
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
}
