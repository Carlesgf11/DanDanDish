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

    public Animator anim;
    public Animator animOK;

    [Header("CargarInfoPlayer")]
    public List<ScriptableCharacters> characters;
    public int selectedChar;

    [Header("Arrow Render Shoot")]
    public GameObject arrowPref;
    public Transform arrowSpawnPos;
    public ArrowControl arrowControl;
    private bool canSpawnArrow;

    private void Start()
    {
        //anim = GetComponentInChildren<Animator>();
        CurrentAction = 0;
        currentCheckpoint = 5;
        ChargePlayersInfo();
    }

    public void ChargePlayersInfo()
    {
        //Cargar informacion de los players
        characters = manager.characters;

        if (IsPlayer1)
            selectedChar = manager.Player1Char;
        else
            selectedChar = manager.Player2Char;

        ps = characters[selectedChar].bodyParts;
        //anim = characters[selectedChar].anim;

        GameObject renderChar = Instantiate(characters[selectedChar].anim, transform);

        if(!IsPlayer1)
            renderChar.GetComponent<SpriteRenderer>().flipX = true;

        anim = GetComponentInChildren<Animator>();
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
        manager.TimelineIsDone = true;
        if (CurrentAction == 1)
        {

            ammo++;
            GameObject newArrow = Instantiate(arrowImage, playerGrid.transform.position, Quaternion.identity);
            newArrow.transform.parent = playerGrid.transform;
            state = PlayerState.ANIMS;
        }
        if (CurrentAction == 2) 
        {
            if (ammo > 0) ammo--;
           
            if (playerGrid.transform.childCount > 0)
                Destroy(playerGrid.transform.GetChild(0).gameObject);
            state = PlayerState.ANIMS;
        }
        if (CurrentAction == 3)
        {
            state = PlayerState.ANIMS;
        }
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
            if (IsPlayer1)
                ShootArrow(Vector3.right, Quaternion.identity);
            else
                ShootArrow(Vector3.right, Quaternion.Euler(0, 0, 180));
        }
        if (CurrentAction == 3)
        {

            anim.SetTrigger("Defend");
        }
    }

    public void ShootArrow(Vector3 _dir, Quaternion _rot)
    {
        if(canSpawnArrow)
        {
            GameObject newArrow = Instantiate(arrowPref, arrowSpawnPos.position, _rot);
            arrowControl = newArrow.GetComponent<ArrowControl>();
            arrowControl.arrowDir = _dir;
            canSpawnArrow = false;
        }
    }

    private void ChooseUpdate()
    {
        canSpawnArrow = true;
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

    public void ButtonChoose(int _action)
    {

        if (IsPlayer1)
        {
            if (_action == 2 && ammo >= 1)
                CurrentAction = _action;
            if (_action != 2)
                CurrentAction = _action;
        }
    }

    public void MoveUpdate()
    {

        Vector2 finalPos = new Vector2(checkPoints[currentCheckpoint].position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, finalPos, 10 * Time.deltaTime);
        float distance = Vector2.Distance(finalPos, transform.position);
        if (distance <= 0.02f)
        {

            if (currentCheckpoint >= 11 )
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
        if (ammo > 0) ammo = 0;
        BorrahFleixas();
        cameraTarget.parent = gameObject.transform;
        cameraTarget.transform.localPosition = new Vector3(cameraX, 3, -10);
        anim.SetTrigger("Run");
        Invoke("GoMove", 1f);

    }
    public void GoMove()
    {

        anim.SetTrigger("Run");
        CurrentAction = 0;
        state = PlayerState.MOVE;
    }
    public void Empate()
    {
        anim.SetTrigger("Idle");
        CurrentAction = 0;
        state = PlayerState.CHOOSE;
    }

    public void Lose()
    {
        Invoke("Die", 0.35f);
        BorrahFleixas();
        CurrentAction = 0;
        if (ammo > 0) ammo = 0;
        print(currentCheckpoint);
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

        //Cambiar pos
        if(currentCheckpoint >= 0)
        {
            Vector2 finalPos = new Vector2(checkPoints[currentCheckpoint].position.x, transform.position.y);
            transform.position = finalPos;
        }
        else
        {
            gameObject.SetActive(false);
            Camera.main.GetComponent<HitStop>().Stop(0.25f);
            SlowMo();
        }
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

    void SlowMo()
    {
        Camera.main.GetComponent<CameraShake>().ShakeIt();
        Time.timeScale = Mathf.Lerp(Time.timeScale, 0.5f, 0.5f);
        Invoke("NoSlowMo", 0.75f);
    }
    void NoSlowMo()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, 0.2f);

    }
}
