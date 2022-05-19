using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { CHOOSE, ACTION, RELOCATE };
    public GameState state;

    public float countDown;
    public Transform cameraTarget;
    public GameObject player1, player2;

    private void Start()
    {
        countDown = 5;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            player1.GetComponent<PlayerControl>().Win();
        if (Input.GetKeyDown(KeyCode.W))
            player1.GetComponent<PlayerControl>().Lose();

        switch (state)
        {
            case GameState.CHOOSE:
                ChooseUpdate();
                break;
            case GameState.ACTION:
                ActionUpdate();
                break;
            case GameState.RELOCATE:
                RelocateUpdate();
                break;
        }
    }

    void ChooseUpdate()
    {
        countDown -= Time.deltaTime;
        if (countDown <= 0)
            state = GameState.ACTION;
    }

    void ActionUpdate()
    {

    }

    void RelocateUpdate()
    {

    }
}
