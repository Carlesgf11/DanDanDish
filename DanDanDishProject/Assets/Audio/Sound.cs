using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    string fontName;
    public string name;
    public AudioClip clip;
    public AudioMixerGroup group;
    [Range(0f, 1f)]
    public float volume; //PONER A .5F. Es el valor intermedio.
    [Range (.1f, 3f)]
    public float pitch; //PONER A 1. Es el valor por defecto
    public bool loop;
    [HideInInspector]
    public AudioSource source;
}
