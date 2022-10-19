using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public Text roomName;
    public InputField roomInputField;


    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemList = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdates = 1.5f;
    public float nextUpdateTime;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;
    private PlayerItem MyPlayer;

    public GameObject btnPlay;

    private void Awake()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
    }
    private void Start()
    {
        PhotonNetwork.JoinLobby();
        btnPlay.SetActive(false);
    }

    private void Update()
    {
        
    }

    public void OnClickCreate()
    {
        if (roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 2, BroadcastPropsChangeToAll = true });
        }
    }
    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemList)
        {
            Destroy(item.gameObject);
        }
        roomItemList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList()
    {
        foreach(PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();
        if (PhotonNetwork.CurrentRoom == null) return;
        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            if(player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }
            playerItemsList.Add(newPlayerItem);
            ChangeOrder();
        }

        if (playerItemsList.Count == 2)
            if (PhotonNetwork.IsMasterClient)
                btnPlay.SetActive(true);
    }

    public void ChangeOrder()
    {
        if (playerItemsList[0].player.IsMasterClient == false)
        {
            playerItemsList[0].name = playerItemsList[0].player.ActorNumber.ToString();
            playerItemsList[0].transform.SetAsLastSibling();
            playerItemsList[0].PlayerImage.transform.localScale = new Vector3(-1,1,1);
        }
        else
        {
            if (playerItemsList.Count > 1)
            {
                if (playerItemsList[1] != null)
                {
                    playerItemsList[1].name = playerItemsList[0].player.ActorNumber.ToString();
                    playerItemsList[1].transform.SetAsLastSibling();
                    playerItemsList[1].PlayerImage.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList(); 
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public void OnClickPlayBtn()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}

