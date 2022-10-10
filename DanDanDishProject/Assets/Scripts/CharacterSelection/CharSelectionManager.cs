using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class CharSelectionManager : MonoBehaviourPunCallbacks
{
    public List<ScriptableCharacters> characters;
    public int currentCharacter;
    public Image charImage;
    public GameObject flagImage;
    public int player;
    PhotonView view;

    [Header("Accesos")]
    public CharSelectionList charList;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    Player playerP;

    void Start()
    {
        
        currentCharacter = 0;
        
        SelectPlayer();
        charList = FindObjectOfType<CharSelectionList>();
        characters = charList.characters;
        view = GetComponent<PhotonView>();  
        view.RPC("PhtnChangeColor", RpcTarget.AllBuffered);
        
    }
    void SelectPlayer() //Identificar ID player
    {
        view = GetComponent<PhotonView>();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            player = 1;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            player = 2;
            transform.GetChild(0).localScale = new Vector3(-1,1,1);
        }
            
    }

    //public void SetPlayerInfo(Player _player)
    //{
    //    playerP = _player;
    //    UpdatePlayerItem(playerP);
    //}

    void Update()
    {
        
    }

    [PunRPC]
    public void PhtnChangeColor()
    {
        charImage.sprite = characters[currentCharacter].charImage;
        flagImage.GetComponent<Animator>().runtimeAnimatorController = characters[currentCharacter].flagAnim;
    }

    public void NextBtn()
    {
        //if((int)playerProperties["charImage"] == characters.Count - 1)
        //{
        //    playerProperties["charImage"] = 0;
        //}else
        //{
        //    playerProperties["charImage"] = (int)playerProperties["charImage"] + 1;
        //}
        //PhotonNetwork.SetPlayerCustomProperties(playerProperties);
        currentCharacter++;
        if (currentCharacter > 2)
            currentCharacter = 0;
        view.RPC("PhtnChangeColor", RpcTarget.AllBuffered);
    }

    public void PrevBtn()
    {
        //if ((int)playerProperties["charImage"] == 0)
        //{
        //    playerProperties["charImage"] = characters.Count - 1;
        //}else
        //{
        //    playerProperties["charImage"] = (int)playerProperties["charImage"] - 1;
        //}
        //PhotonNetwork.SetPlayerCustomProperties(playerProperties);
        currentCharacter--;
        if (currentCharacter < 0)
            currentCharacter = 2;
        view.RPC("PhtnChangeColor", RpcTarget.AllBuffered);
    }

    //public override void OnPlayerPropertiesUpdate(Player _targetPlayer, ExitGames.Client.Photon.Hashtable _playerProperties)
    //{
    //    if(playerP == _targetPlayer)
    //    {
    //        UpdatePlayerItem(_targetPlayer);
    //    }
    //}

    //void UpdatePlayerItem(Player _player)
    //{
    //    if(_player.CustomProperties.ContainsKey("charImage"))
    //    {
    //        charImage.sprite = characters[(int)_player.CustomProperties["charImage"]].charImage;
    //        playerProperties["charImage"] = (int)_player.CustomProperties["charImage"];
    //    }else
    //    {
    //        playerProperties["charImage"] = 0;
    //    }
    //}

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
