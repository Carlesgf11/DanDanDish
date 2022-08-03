using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState { CHOOSE, ACTION, RELOCATE, GAMEFINISHED };
    public GameState state;

    public float countDown;
    public Transform cameraTarget;
    public GameObject player1, player2;

    public Text countDownText;

    public GameObject winnerPanel;
    public Text winnerText;

    [Header("CargarInfoPlayers")]
    public int Player1Char, Player2Char;
    public List<ScriptableCharacters> characters;

    [Header("Pause")]
    public GameObject pausePanel;
    public GameObject pauseContentPanel;
    public GameObject optionsPanel;

    private void Start()
    {
        countDown = 4;
        pauseContentPanel.SetActive(true);
        winnerPanel.SetActive(false);
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        Player1Char = PlayerPrefs.GetInt("Player1", 0);
        Player2Char = PlayerPrefs.GetInt("Player2", 0);
        //print(Player1Char);
        //print(Player2Char);
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Alpha1))
        //    player1.GetComponent<PlayerControl>().Win();
        //
        //if(Input.GetKeyDown(KeyCode.Alpha2))
        //    player2.GetComponent<PlayerControl>().Win();
        //
        //if (Input.GetKeyDown(KeyCode.W))
        //    player1.GetComponent<PlayerControl>().Lose();
        //
        //if (Input.GetKeyDown(KeyCode.Q))
        //    player2.GetComponent<PlayerControl>().Lose();

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
            case GameState.GAMEFINISHED:
                //winnerText.text = 
                break;
        }
    }

    void ChooseUpdate()
    {
        countDown -= Time.deltaTime;
        countDownText.text = ((int)countDown).ToString();
        if (countDown <= 1)
            state = GameState.ACTION;
    }

    void ActionUpdate()
    {
        int _Player1 = player1.GetComponent<PlayerControl>().CurrentAction;
        int _Player2 = player2.GetComponent<PlayerControl>().CurrentAction;
        if (_Player1 == _Player2)
        {
            countDown = 4;
            Invoke("ReturnToChoose", 1f);
        }else if(_Player1 < _Player2 && _Player2 != 3 && _Player2 != 1)
        {
            Player2Win();
        }
        else if (_Player1 > _Player2 && _Player1 != 3 && _Player1 != 1)
        {
            Player1Win();
        }
        else if (_Player1 < _Player2 && _Player2 != 2)
        {
            countDown = 4;
            Invoke("ReturnToChoose", 1f);
        }
        else if (_Player1 > _Player2 && _Player1 != 2)
        {
            countDown = 4;
            Invoke("ReturnToChoose", 1f);
        }
    }

    public void Player1Win()
    {
        player1.GetComponent<PlayerControl>().currentCheckpoint++;
        player2.GetComponent<PlayerControl>().currentCheckpoint--;
        player1.GetComponent<PlayerControl>().Win();
        player2.GetComponent<PlayerControl>().Lose();
        state = GameState.RELOCATE;
    }

    public void Player2Win()
    {
        player2.GetComponent<PlayerControl>().currentCheckpoint++;

        player1.GetComponent<PlayerControl>().currentCheckpoint--;
        player2.GetComponent<PlayerControl>().Win();
        player1.GetComponent<PlayerControl>().Lose();
        state = GameState.RELOCATE;
    }

    public void ReturnToChoose()
    {
        player1.GetComponent<PlayerControl>().Empate();
        player2.GetComponent<PlayerControl>().Empate();
        player1.GetComponent<PlayerControl>().CurrentAction = 0;
        player2.GetComponent<PlayerControl>().CurrentAction = 0;
        state = GameState.CHOOSE;
    }

    public void FinishGame(GameObject _winner)
    {
        winnerPanel.SetActive(true);
        winnerText.text = _winner.name + " wins";
        print(_winner + "Wins");
        state = GameState.GAMEFINISHED;
    }

    void RelocateUpdate()
    {
        
    }

    #region Pause
    public void OnApplicationPause(bool _pause)
    {
        if(_pause)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        }
    }

    public void PauseON()
    {
        OnApplicationPause(true);
    }
    public void PauseOFF()
    {
        OnApplicationPause(false);
    }
    #endregion
}
