using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/New Character")]
public class ScriptableCharacters : ScriptableObject
{
    public int index;
    public Sprite charImage;
    public GameObject bodyParts;
    public GameObject anim;
}

