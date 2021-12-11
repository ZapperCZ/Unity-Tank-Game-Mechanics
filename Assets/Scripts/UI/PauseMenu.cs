using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    [SerializeField] GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Pause Key"))
        {
            isPaused = !isPaused;
            ChangeGameState(isPaused);
        }
    }
    public void ChangeGameState(bool pauseGame)
    {
        pauseMenuUI.SetActive(pauseGame);
        Time.timeScale = (float)Convert.ToInt32(!pauseGame);
        Cursor.lockState = pauseGame ? CursorLockMode.None : CursorLockMode.Locked;
    }
    public void ReloadMainScene()
    {
        isPaused = false;
        ChangeGameState(isPaused);
        SceneManager.LoadScene("SampleScene");
    }
    public void QuitGame()
    {

    }
}
