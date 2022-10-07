using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public InputField createInput, joinInpt;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInpt.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("CharacterSelection");
    }
}
