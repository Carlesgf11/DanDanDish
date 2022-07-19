using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharSelectionManager : MonoBehaviour
{
    public List<ScriptableCharacters> characters;
    public int currentCharacter;
    public Image charImage;
    public int player;

    [Header("Accesos")]
    public CharSelectionList charList;

    void Start()
    {
        currentCharacter = 0;
        characters = charList.characters;
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

    public void SendCharInt(string _keyName1, string _keyName2)
    {
        if(player == 1)
            PlayerPrefs.SetInt(_keyName1, currentCharacter);
        else if(player == 2)
            PlayerPrefs.SetInt(_keyName2, currentCharacter);
    }

    public void ToGameScene()
    {
        SendCharInt("Player1", "Player2");
        SceneManager.LoadScene(2);
    }
}
