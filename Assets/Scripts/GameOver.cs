using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void Retry()
    {
        SceneManager.LoadScene(1);
    }
     void Start()
     {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
     }
}
