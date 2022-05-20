using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public enum PlayerState { CHOOSE, ACTION, MOVE, DIE };
    public PlayerState state;
    public GameManager manager;
    public List<Transform> checkPoints;
    public int currentCheckpoint;
    public bool IsPlayer1;
    public Transform cameraTarget;
    public int cameraX;
    public int CurrentAction;//1Recargar/ 2Disparar/ 3Defender

    Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        currentCheckpoint = 5;
    }

    private void Update()
    {
        switch (state)
        {
            case PlayerState.CHOOSE:
                ChooseUpdate();
                break;
            case PlayerState.ACTION:
                ActionUpate();
                break;
            case PlayerState.MOVE:
                MoveUpdate();
                break;
            case PlayerState.DIE:
                break;
        }
    }

    public  void ActionUpate()
    {
        if (CurrentAction == 1) anim.SetTrigger("Recharge");
        if (CurrentAction == 2) anim.SetTrigger("Shoot");
        if (CurrentAction == 3) anim.SetTrigger("Defend");
    }

    private void ChooseUpdate()
    {
        if (manager.state == GameManager.GameState.CHOOSE)
        {
            if (IsPlayer1)
            {
                if (Input.GetKeyDown(KeyCode.A)) CurrentAction = 1;
                if (Input.GetKeyDown(KeyCode.S)) CurrentAction = 2;
                if (Input.GetKeyDown(KeyCode.D)) CurrentAction = 3;
            }
            if (!IsPlayer1)
            {
                if (Input.GetKeyDown(KeyCode.J)) CurrentAction = 1;
                if (Input.GetKeyDown(KeyCode.K)) CurrentAction = 2;
                if (Input.GetKeyDown(KeyCode.L)) CurrentAction = 3;
            }
        }
        else
        {
            state = PlayerState.ACTION;
        }
    }

    public void MoveUpdate()
    {
        Vector2 finalPos = new Vector2(checkPoints[currentCheckpoint].position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, finalPos, 10 * Time.deltaTime);
        float distance = Vector2.Distance(finalPos, transform.position);
        if (distance <= 0.02f)
        {
            transform.position = finalPos;
            anim.SetTrigger("Idle");
            manager.ReturnToChoose();
            state = PlayerState.CHOOSE;
        }
    }

    public void Win()
    {
        //currentCheckpoint++;
        cameraTarget.parent = gameObject.transform;
        cameraTarget.transform.localPosition = new Vector3(cameraX, 3, -10);
        anim.SetTrigger("Run");
        Invoke("GoMove", 1f);
    }
    public void GoMove()
    {
        state = PlayerState.MOVE;
    }
    public void Empate()
    {
        anim.SetTrigger("Idle");
        state = PlayerState.CHOOSE;
    }

    public void Lose()
    {
        //currentCheckpoint--;
        Vector2 finalPos = new Vector2(checkPoints[currentCheckpoint].position.x, transform.position.y);
        transform.position = finalPos;
        Invoke("Empate", 1);
    }
}
