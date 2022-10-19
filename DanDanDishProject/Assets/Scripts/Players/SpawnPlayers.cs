using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public GameManager gameManager;

    public List<GameObject> playerItemsList = new List<GameObject>();

    //public ScriptableCharacters characters; 
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    private bool leave;

    private void Start()
    {
        leave = false;
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        int tempPos = 0;
        foreach (GameObject item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();
        if (PhotonNetwork.CurrentRoom == null) return;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            GameObject newPlayer = Instantiate(playerPrefab, spawnPoints[tempPos]);

            newPlayer.gameObject.name = newPlayer.gameObject.name + tempPos.ToString();
            newPlayer.transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = newPlayer.gameObject.name;

            PlayerControl newPlayerControl = newPlayer.transform.GetChild(0).GetComponent<PlayerControl>();

            newPlayerControl.SetPlayerInfo(player.Value);

            if (newPlayerControl.player.IsMasterClient == true)
                print(newPlayer.transform.name + "Soy master");
            else
                print(newPlayer.transform.name + "No soy master");

            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerControl.ApplyLocalChanges();
            }
            playerItemsList.Add(newPlayer);
            tempPos++;
        }
        ChangeOrder();
    }

    public void ChangeOrder()
    {
        if (spawnPoints[0].transform.GetChild(0).transform.GetChild(0).GetComponent<PlayerControl>().player.IsMasterClient == false)
        {
            spawnPoints[0].transform.GetChild(0).transform.position = spawnPoints[1].transform.position;
            spawnPoints[0].transform.GetChild(0).transform.parent = spawnPoints[1].transform;

            spawnPoints[1].transform.GetChild(0).transform.position = spawnPoints[0].transform.position;
            spawnPoints[1].transform.GetChild(0).transform.parent = spawnPoints[0].transform;
        }
        spawnPoints[1].transform.GetChild(0).transform.localScale = new Vector3(-1, 1, 1);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public void AutoLeaveRoom()
    {
        
    }

    public void ChangeMaster()
    {
        //spawnPoints[0].transform.GetChild(0).transform.GetChild(0).GetComponent<PlayerControl>().player.IsMasterClient == false 
    }
}
