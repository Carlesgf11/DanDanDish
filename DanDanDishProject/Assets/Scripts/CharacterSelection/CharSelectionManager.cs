using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelectionManager : MonoBehaviour
{
    public List<ScriptableCharacters> characters;
    public int currentCharacter;
    public Image charImage;

    void Start()
    {
        currentCharacter = 0;
    }

    void Update()
    {
        charImage.sprite = characters[currentCharacter].charImage;
    }

    public void NextBtn()
    {
        currentCharacter++;
        if (currentCharacter > 2)
            currentCharacter = 0;
    }
    public void PrevBtn()
    {
        currentCharacter--;
        if (currentCharacter < 0)
            currentCharacter = 2;
    }
}
