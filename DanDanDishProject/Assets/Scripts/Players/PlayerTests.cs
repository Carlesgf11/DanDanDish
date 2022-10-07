using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerTests : MonoBehaviour
{
    private float speed = 10;
    PhotonView view;


    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(view.IsMine)
        {
            transform.Translate(Vector2.right * speed * Input.GetAxis("Horizontal") * Time.deltaTime);
            transform.Translate(Vector2.up * speed * Input.GetAxis("Vertical") * Time.deltaTime);
        }
    }
}
