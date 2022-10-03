using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider generalSlider, musicSlider, sfxSlider;

    public const string MIXER_GENERAL = "GeneralVolume";
    public const string MIXER_MUSIC = "MusicVolume";
    public const string MIXER_SFX = "SFXVolume";

    private void Awake()
    {
        generalSlider.onValueChanged.AddListener(SetGeneralVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void Start()
    {
        generalSlider.value = PlayerPrefs.GetFloat(AudioManager.GENERAL_KEY, 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY, 0.5f);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(AudioManager.GENERAL_KEY, generalSlider.value);
        PlayerPrefs.SetFloat(AudioManager.MUSIC_KEY, musicSlider.value);
        PlayerPrefs.SetFloat(AudioManager.SFX_KEY, sfxSlider.value);
    }

    void SetGeneralVolume(float _value)
    {
        mixer.SetFloat(MIXER_GENERAL, Mathf.Log10(_value) * 20); //MathF: Combertir el valor a logaritmico y multiplicamos por 20 para poder llegar a -80 dB
    }
    void SetMusicVolume(float _value)
    {
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(_value) * 20); 
    }
    void SetSFXVolume(float _value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(_value) * 20);
    }
}
