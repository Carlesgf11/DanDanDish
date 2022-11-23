using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text DebugText; // ---------------------------------- DEBUG TEXT -------------------------------------------------
    //Prueba Commit
    public Animator ButtonsAnim;
    public List<GameObject> flagsImages;
    public enum GameState { CHOOSE, DELAYTOACTION, ACTION, RELOCATE, GAMEFINISHED };
    public GameState state;
    public bool TimelineIsDone = false;
    bool TimelineOutDone = false;
    public float countDown, delayTimer;
    public Transform cameraTarget;

    public Transform[] playersInstSpots;
    public GameObject player1, player2;

    public Text countDownText;

    [Header("CargarInfoPlayers")]
    public int Player1Char, Player2Char;
    public List<ScriptableCharacters> characters;

    [Header("Pause")]
    public GameObject pausePanel;
    public bool pause;
    public GameObject pauseContentPanel;
    public GameObject optionsPanel;

    [Header("Sounds")]
    public AudioManager audioManager;

    PhotonView view;

    bool isChoosing;

    [Header("End")]
    [SerializeField] GameObject structureParts;
    [SerializeField] Transform structurePos1, structurePos2;
    [SerializeField] GameObject structure1, structure2;
    public GameObject winnerPanel;
    public Text winnerText;
    [SerializeField] bool winnerPlayer1;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        SetGame();
    }

    private void SetGame()
    {
        pause = false;
        player1 = playersInstSpots[0].transform.GetChild(0).transform.GetChild(0).gameObject;
        player2 = playersInstSpots[1].transform.GetChild(0).transform.GetChild(0).gameObject;

        countDown = 4;
        pauseContentPanel.SetActive(true);
        winnerPanel.SetActive(false);
        pausePanel.SetActive(false);
        //optionsPanel.SetActive(false); //Si se hace este set active se activa la funcion onDisable del VolumeSetting y se pone chungo todo
        Player1Char = PlayerPrefs.GetInt("Player1", 0);
        Player2Char = PlayerPrefs.GetInt("Player2", 0);
        //print(Player1Char);
        //print(Player2Char);
        for (int i = 0; i < flagsImages.Count; i++)
        {
            flagsImages[i].GetComponent<Image>().sprite = characters[Player1Char].flagSprite;
            flagsImages[i].transform.GetChild(0).GetComponent<Image>().color = characters[Player1Char].UIColor;
        }
    }

    private void Update()
    {
        #region Shortcuts
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //    Player1Win();
        //player1.GetComponent<PlayerControl>().Win();
        //
        //if(Input.GetKeyDown(KeyCode.Alpha2))
        //    player2.GetComponent<PlayerControl>().Win();
        //
        //if (Input.GetKeyDown(KeyCode.W))
        //    player1.GetComponent<PlayerControl>().Lose();
        //
        //if (Input.GetKeyDown(KeyCode.Q))
        //    player2.GetComponent<PlayerControl>().Lose();
        #endregion

        switch (state)
        {
            case GameState.CHOOSE:
                ChooseUpdate();
                break;
            case GameState.DELAYTOACTION:
                GoAction();
                break;
            case GameState.ACTION:
                ActionUpdate();
                break;
            case GameState.RELOCATE:
                RelocateUpdate();
                break;
            case GameState.GAMEFINISHED:
                break;
        }
    }

    void ChooseUpdate()
    {
        countDown -= Time.deltaTime;
        countDownText.text = ((int)countDown).ToString();
        player1.GetComponent<PlayerControl>().ButtonsAnim.SetBool("Appear", true);
        player2.GetComponent<PlayerControl>().ButtonsAnim.SetBool("Appear", true);

        if (countDown < 1)
        {
            countDown = 0;
            delayTimer = 0.4f;
            player1.GetComponent<PlayerControl>().ButtonsAnim.SetBool("Appear", false);
            player2.GetComponent<PlayerControl>().ButtonsAnim.SetBool("Appear", false);            
            state = GameState.DELAYTOACTION;
        }
    }
    void GoAction()
    {
        delayTimer -= Time.deltaTime;
        if(delayTimer <= 0)
        {
            isChoosing = true;
            delayTimer = 0;
            state = GameState.ACTION;
        }       
    }

    void ActionUpdate()
    {
        if(isChoosing)
        {
            int _Player1 = player1.GetComponent<PlayerControl>().CurrentAction;
            int _Player2 = player2.GetComponent<PlayerControl>().CurrentAction;
            //int _Player2 = player2.GetComponent<PlayerControl>().CurrentAction;
            //0 = nada
            //1 = recargar
            //2 = disparar
            //3 = defenderse
            if (_Player1 == _Player2)
            {
                if (_Player1 == 3)
                    audioManager.PlaySound("DobleShield");
                Invoke("ReturnToChoose", 1f);
                //DebugText.text = (_Player1 + " - " + _Player2 + "\n" + "EMPATE").ToString();
            }
            else if (_Player1 < _Player2 && _Player2 != 3 && _Player2 != 1)
            {
                Player2Win();
                //DebugText.text = (_Player1 + " - " + _Player2 + "\n" + "Player 2 WIN").ToString();
                player1.GetComponent<PlayerControl>().BorrahFleixas();
                player2.GetComponent<PlayerControl>().BorrahFleixas();
            }
            else if (_Player1 > _Player2 && _Player1 != 3 && _Player1 != 1)
            {
                Player1Win();
                //DebugText.text = (_Player1 + " - " + _Player2 + "\n" + "Player 1 WIN").ToString();
                player1.GetComponent<PlayerControl>().BorrahFleixas();
                player2.GetComponent<PlayerControl>().BorrahFleixas();
            }
            else if (_Player1 < _Player2 && _Player2 != 2)
            {
                //if (_Player2 != 0)
                //    audioManager.PlaySound("ArrowImpact_Wood");

                Invoke("ReturnToChoose", 1f);
                //DebugText.text = (_Player1 + " - " + _Player2 + "\n" + "EMPATE").ToString();
            }
            else if (_Player1 > _Player2 && _Player1 != 2)
            {
                //if(_Player1 != 0)
                //    audioManager.PlaySound("ArrowImpact_Wood");

                Invoke("ReturnToChoose", 1f);
                //DebugText.text = (_Player1 + " - " + _Player2 + "\n" + "EMPATE").ToString();
            }
            isChoosing = false;
        }
        EventSystem.current.SetSelectedGameObject(null);  
    }

    

    public void Player1Win()
    {
        state = GameState.RELOCATE;
        player1.GetComponent<PlayerControl>().currentCheckpoint++;
        player2.GetComponent<PlayerControl>().currentCheckpoint--;
        player1.GetComponent<PlayerControl>().Win();
        player2.GetComponent<PlayerControl>().Lose();
        audioManager.PlaySound("arrowImpact");
    }

    public void Player2Win()
    {
        state = GameState.RELOCATE;
        player2.GetComponent<PlayerControl>().currentCheckpoint++;
        player1.GetComponent<PlayerControl>().currentCheckpoint--;
        player2.GetComponent<PlayerControl>().Win();
        player1.GetComponent<PlayerControl>().Lose();
        audioManager.PlaySound("arrowImpact");
    }

    public void ReturnToChoose()
    {
        countDown = 4;
        state = GameState.CHOOSE;
        player1.GetComponent<PlayerControl>().Empate();// El current acction ya se resetea en esta funcion.
        player2.GetComponent<PlayerControl>().Empate();// El current acction ya se resetea en esta funcion.     
    }

    public void FinishGame(GameObject _winner, bool _isPlayer1)
    {
        winnerPanel.SetActive(true);
        winnerText.text = _winner.name + " wins";
        print(_winner + "Wins");
        //Instanciar partes de la estructura
        //InstStructureBreak(_isPlayer1);
        state = GameState.GAMEFINISHED;
    }

    public void InstStructureBreak(bool _isPlayer1)
    {
        Vector3 finalPos = new Vector3(0, 0, 0);
        int finaldir = 1;

        if (_isPlayer1)
        {
            finalPos = structurePos1.position;
            finaldir = -1;
            Destroy(structure1);
        }
        if (!_isPlayer1)
        {
            finalPos = structurePos2.position;
            Destroy(structure2);
        }

        GameObject structurePartsTemp = Instantiate(structureParts, finalPos, Quaternion.identity);
        structurePartsTemp.transform.localScale = new Vector3(finaldir, 1, 1);
    }

    void RelocateUpdate()
    {
        
    }

    #region Pause
    public void OnApplicationPause(bool _pause)
    {
        if(_pause)
        {
            pause = true;
            pausePanel.SetActive(true);
        }
        else
        {
            pause = false;
            pausePanel.SetActive(false);
        }
    }
    #endregion
}
