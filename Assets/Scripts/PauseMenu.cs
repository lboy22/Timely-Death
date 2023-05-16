using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
         if (Time.timeScale == 0 && Input.GetKeyDown(KeyCode.P))
        {
            ResumeGame();
        }       
        else if (Time.timeScale == 1 && Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
    }
    void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
