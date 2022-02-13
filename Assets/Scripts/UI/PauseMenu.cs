using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] CursorLockMode LockModeOnUnpause;

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
        Cursor.lockState = pauseGame ? CursorLockMode.None : LockModeOnUnpause;
    }
    public void ReloadCurrentScene()
    {
        isPaused = false;
        ChangeGameState(isPaused);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
