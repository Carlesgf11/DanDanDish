using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerTests : MonoBehaviour
{
    private float speed = 10;
    PhotonView view;
    public int player;

    private void Start()
    {
        SelectPlayer();
    }

    private void Update()
    {
        if(view.IsMine)
        {
            transform.Translate(Vector2.right * speed * Input.GetAxis("Horizontal") * Time.deltaTime);
            transform.Translate(Vector2.up * speed * Input.GetAxis("Vertical") * Time.deltaTime);
        }
    }
    void SelectPlayer()
    {
        view = GetComponent<PhotonView>();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            player = 1;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            player = 2;
    }
}
