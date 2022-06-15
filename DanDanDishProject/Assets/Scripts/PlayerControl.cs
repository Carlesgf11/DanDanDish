using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public enum PlayerState { CHOOSE, ACTION, ANIMS ,MOVE, DIE };
    public PlayerState state;
    public GameManager manager;
    public List<Transform> checkPoints;
    public int currentCheckpoint;
    public bool IsPlayer1;
    public Transform cameraTarget;
    public int cameraX;
    public int CurrentAction;//1Recargar/ 2Disparar/ 3Defender
    public int ammo;
    public GameObject ps, blood;
    public float fieldOfImpact, force;
    public LayerMask layerToHit;

    public GameObject arrowImage;
    public GameObject playerGrid;

    Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        CurrentAction = 3;
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
            case PlayerState.ANIMS:
                AnimsUpdate();
                break;
            case PlayerState.MOVE:
                MoveUpdate();
                break;
            case PlayerState.DIE:
                break;
        }
    }

    public void ActionUpate()
    {
        if (CurrentAction == 1)
        {
            ammo++;
            GameObject newArrow = Instantiate(arrowImage, playerGrid.transform.position, Quaternion.identity);
            newArrow.transform.parent = playerGrid.transform;
            state = PlayerState.ANIMS;
        }
        if (CurrentAction == 2) 
        {
            ammo--;
            if(playerGrid.transform.childCount > 0)
                Destroy(playerGrid.transform.GetChild(0).gameObject);
            state = PlayerState.ANIMS;
        }
        if (CurrentAction == 3) state = PlayerState.ANIMS;
    }

    public void AnimsUpdate()
    {
        if (CurrentAction == 1)
        {
            anim.SetTrigger("Recharge");
        }
        if (CurrentAction == 2)
        {
            anim.SetTrigger("Shoot");
        }
        if (CurrentAction == 3) anim.SetTrigger("Defend");
    }

    private void ChooseUpdate()
    {
        if (manager.state == GameManager.GameState.CHOOSE || manager.state == GameManager.GameState.RELOCATE)
        {
            if (IsPlayer1)
            {
                if (Input.GetKeyDown(KeyCode.A)) CurrentAction = 1;
                if (Input.GetKeyDown(KeyCode.S) && ammo >= 1) CurrentAction = 2;
                if (Input.GetKeyDown(KeyCode.D)) CurrentAction = 3;
            }
            if (!IsPlayer1)
            {
                if (Input.GetKeyDown(KeyCode.J)) CurrentAction = 1;
                if (Input.GetKeyDown(KeyCode.K) && ammo >= 1) CurrentAction = 2;
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
            if(currentCheckpoint >= 6)
            {
                manager.FinishGame(gameObject);
                return;
            }
            transform.position = finalPos;
            anim.SetTrigger("Idle");
            manager.ReturnToChoose();
            state = PlayerState.CHOOSE;
        }
    }

    public void Win()
    {
        //currentCheckpoint++;
        ammo = 0;
        BorrahFleixas();
        cameraTarget.parent = gameObject.transform;
        cameraTarget.transform.localPosition = new Vector3(cameraX, 3, -10);
        anim.SetTrigger("Run");
        Invoke("GoMove", 1f);
    }
    public void GoMove()
    {
        anim.SetTrigger("Run");
        CurrentAction = 3;
        state = PlayerState.MOVE;
    }
    public void Empate()
    {
        anim.SetTrigger("Idle");
        CurrentAction = 3;
        state = PlayerState.CHOOSE;
    }

    public void Lose()
    {
        Die();
        BorrahFleixas();
        //currentCheckpoint--;
        CurrentAction = 3;
        ammo = 0;
        Vector2 finalPos = new Vector2(checkPoints[currentCheckpoint].position.x, transform.position.y);
        transform.position = finalPos;
        Invoke("Empate", 1);
    }
    public void Die()
    {
        Vector2 pos = new Vector2(transform.position.x, (transform.position.y - (UnityEngine.Random.Range(0f, 1f))));
        Instantiate(blood, pos, Quaternion.identity);
        Instantiate(ps, transform.position, ps.transform.rotation);
        Collider2D[] objects = Physics2D.OverlapCircleAll(pos, fieldOfImpact, layerToHit);
        foreach (Collider2D obj in objects)
        {
            Vector2 direction = (Vector2)obj.transform.position - pos;
            obj.GetComponent<Rigidbody2D>().AddForce(direction * force * 100f);

        }
        ps.transform.SetParent(null);
    }

    public void BorrahFleixas()
    {
        //Funcion patrocinada por Albert
        for (int i = playerGrid.transform.childCount - 1; i >= 0; i--)
        {
            if (playerGrid.transform.childCount > 0)
                Destroy(playerGrid.transform.GetChild(i).gameObject);
            else
                break;
        }
    }
}
