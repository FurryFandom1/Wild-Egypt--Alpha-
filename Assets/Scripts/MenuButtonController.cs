using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour
{

    [SerializeField] public AudioSource _audioSource;
    [SerializeField] public GameObject mainButtons;
    [SerializeField] public GameObject settings;
    [SerializeField] public GameObject playButtons;
    [SerializeField] public GameObject multiplayerMenu;

    public void Play()
    {
        SceneManager.LoadScene(2);
    }

    public void Exit()
    {

        StartCoroutine(leave());
    }

    IEnumerator leave()
    {
        yield return new WaitForSeconds(2);
        Application.Quit();
    }
    
    public void Settings()
    {
        mainButtons.SetActive(false);
        settings.SetActive(true);
    }
    
    public void ChooseGameMode()
    {
        mainButtons.SetActive(false);
        playButtons.SetActive(true);
    }
    public void ChooseGameModeBack()
    {
        mainButtons.SetActive(true);
        playButtons.SetActive(false);
    }
    public void MultiplayerMenu()
    {
        playButtons.SetActive(false);
        multiplayerMenu.SetActive(true);
    }
    public void MultiplayerMenuBack()
    {
        playButtons.SetActive(true);
        multiplayerMenu.SetActive(false);
    }
    
}
        