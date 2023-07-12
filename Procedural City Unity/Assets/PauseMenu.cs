using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool paused = false;
    public ProceduralScript proceduralScript;
    public PlayerController playerController;

    float originalTimeScale; 

    void Awake()
    {
        originalTimeScale = Time.timeScale;
        proceduralScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<ProceduralScript>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialiser(bool pauseState)
    {
        if (pauseState)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
            //proceduralScript.timerPaused = true;
            playerController.active = false;
            playerController.paused = true;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //proceduralScript.timerPaused = false;
            Time.timeScale = 1;
            playerController.active = true;
            playerController.paused = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void Resume()
    {
        Initialiser(false);
    }    

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
