using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public GameObject UI;
    public static bool GameIsStopped = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (GameIsStopped)
            {
                Resume();
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Pause();
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public void Resume()
    {
        UI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        GameIsStopped = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        GameIsStopped = false;
        SceneManager.LoadScene("MainScene");
    }

    public void Pause()
    {
        UI.SetActive(true);
        Time.timeScale = 0f;
        GameIsStopped = true;
    }

    public void QuitGame()
    {
        
        Application.Quit();
    }
}
