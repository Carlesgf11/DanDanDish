using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public InputField createInput, joinInpt;

    public void CreateRoom()
    {
        if(createInput.text.Length >= 1)
            PhotonNetwork.CreateRoom(createInput.text, new RoomOptions() { MaxPlayers = 2, BroadcastPropsChangeToAll = true });
    }

    public void JoinRoom()
    {
        if (joinInpt.text.Length >= 1)
            PhotonNetwork.JoinRoom(joinInpt.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("CharacterSelection");
    }
}
