using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneManagement : MonoBehaviour
{
    public void ChangeScene(int _scene) { SceneManager.LoadScene(_scene); }
    public void ExitGame() { Application.Quit(); }
}
