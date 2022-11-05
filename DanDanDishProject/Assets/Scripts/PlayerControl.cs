using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviourPunCallbacks
{
    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    PhotonView view;

    public Player player;
    public GameManager manager;
    public enum PlayerState { CHOOSE, ACTION, ANIMS, MOVE, DIE };
    public PlayerState state;

    public List<Transform> checkPoints;
    public bool IsPlayer1;
    public Transform cameraTarget;
    public LayerMask layerToHit;
    public PlayerControl opponent;
    public GameObject playerGrid;
    public Animator anim;

    //BOTONES UI
    public Animator ButtonsAnim;
    public List<GameObject> flagsImages;
    public GameObject actionsBtns;

    //INTS
    public int currentCheckpoint;
    public int cameraX;
    public int CurrentAction;//1Recargar/ 2Disparar/ 3Defender
    //public int currentActionOpponent;
    public int ammo;
    public GameObject ps, blood;
    public float fieldOfImpact, force;
    private Transform[] playersInstSpots;

    [Header("CargarInfoPlayer")]
    public List<ScriptableCharacters> characters;
    public int selectedChar;

    [Header("Arrow Render Shoot")]
    public GameObject arrowImage;
    public GameObject arrowPref;
    public Transform arrowSpawnPos;
    public ArrowControl arrowControl;
    private bool canSpawnArrow;
    public GameObject SpriteJugador;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        AutoLeave();
        currentCheckpoint = 5;
        ChargePlayersInfo();
        FindOpponent();
    }

    public void FindOpponent()
    {
        playersInstSpots = manager.playersInstSpots;
        if (transform.parent.transform.parent == playersInstSpots[0])
            opponent = playersInstSpots[1].transform.GetChild(0).transform.GetChild(0).GetComponent<PlayerControl>();
        else if (transform.parent.transform.parent == playersInstSpots[1])
            opponent = playersInstSpots[0].transform.GetChild(0).transform.GetChild(0).GetComponent<PlayerControl>();
    }

    public void AutoLeave()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            FindObjectOfType<SpawnPlayers>().OnClickLeaveRoom();    
        }
    }

    public void SetPlayerInfo(Player _player)
    {
        //playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(player);
    }

    public void ApplyLocalChanges()
    {
        //actionsBtns.SetActive(true);
        ButtonsAnim.gameObject.SetActive(true);
    }

    void UpdatePlayerItem(Player player)
    {
        if (player.CustomProperties.ContainsKey("CurrentAction"))
        {
            CurrentAction = (int)player.CustomProperties["CurrentAction"];
        }
        else
        {
            playerProperties["CurrentAction"] = 0;
        }
    }

    public void ChargePlayersInfo()
    {
        //Cargar informacion de los players
        manager = FindObjectOfType<GameManager>();
        cameraTarget = manager.cameraTarget;
        characters = manager.characters;

        selectedChar = (int)player.CustomProperties["playerAvatar"];
        ps = characters[(int)player.CustomProperties["playerAvatar"]].bodyParts;

        GameObject renderChar = Instantiate(characters[selectedChar].anim, transform);
        anim = renderChar.GetComponent<Animator>();

        for (int i = 0; i < flagsImages.Count; i++)
        {
            flagsImages[i].GetComponent<Image>().sprite = characters[(int)player.CustomProperties["playerAvatar"]].flagSprite;
            flagsImages[i].transform.GetChild(0).GetComponent<Image>().color = characters[(int)player.CustomProperties["playerAvatar"]].UIColor;
        }
    }

    private void Update()
    {
        //currentActionOpponent = opponent.CurrentAction;

        switch (state)
        {
            case PlayerState.CHOOSE:
                ChooseUpdate();
                //currentActionOpponent = opponent.CurrentAction;
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
        if (manager.state != GameManager.GameState.CHOOSE || manager.state != GameManager.GameState.RELOCATE)
        {
            state = PlayerState.ACTION;
        }
    }
  
    public void ButtonChoose(int _action)
    {
        if (_action == 2 && ammo >= 1 || _action != 2) //PARA DISPARAR
        {
            playerProperties["CurrentAction"] = _action;
        }
        else
        {
            playerProperties["CurrentAction"] = 0;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }
    #region MOVEMENT
    public void MoveUpdate()
    {
        ButtonsAnim.SetBool("Appear", false);
        Vector2 finalPos = new Vector2(checkPoints[currentCheckpoint].position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, finalPos, 10 * Time.deltaTime);
        float distance = Vector2.Distance(finalPos, transform.position);
        if (distance <= 0.02f)
        {
            if (currentCheckpoint >= 11)
            {
                manager.FinishGame(gameObject);
                return;
            }
            transform.position = finalPos;
            anim.SetTrigger("Idle");
            manager.ReturnToChoose(); //RETURN TO CHOOSE
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
        state = PlayerState.MOVE;
        CurrentAction = 0;
    }

    public void Empate()
    {
        anim.SetTrigger("Idle");
        state = PlayerState.CHOOSE;
        CurrentAction = 0;
    }

    public void Lose()
    {
        Invoke("Die", 0.35f);
        BorrahFleixas();
        if (ammo > 0) ammo = 0;
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
            Camera.main.GetComponent<HitStop>().Stop(0.3f);
            SlowMo();
            gameObject.SetActive(false);
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
        ammo = 0;
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
    #endregion
}
