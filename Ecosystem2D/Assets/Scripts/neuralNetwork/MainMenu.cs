using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        World.Active.GetExistingSystem<BibitFieldMeasurementSystem>().Enabled = false;    
    }

    public void playGame()
    {
        Debug.Log("load game Scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void exitApplication()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
