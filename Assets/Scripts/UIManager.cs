using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject mainMenu;

    private bool paused = false;


    void Start() { }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PauseGame(!paused);
        }
    }

    public void PauseGame(bool pause = true)
    {
        Time.timeScale = (paused = pause) ? 0 : 1;
        pauseMenu.SetActive(paused);
    }
}
