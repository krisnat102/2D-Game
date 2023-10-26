using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject Menu;

    public GameObject Settings;

    void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            if (Menu.activeSelf)
            {
                UnpauseGame();
            }
            else
            {
                Menu.SetActive(true);

                GameManager.gamePaused = true;
                PauseGame();
            }
        }
    }
    void PauseGame()
    {
        if (GameManager.gamePaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void UnpauseGame()
    {
        Menu.SetActive(false);

        GameManager.gamePaused = false;
        PauseGame();
    }

    public void StartSettings()
    {
        Settings.SetActive(true);
    }

    public void MainMenu()
    {
        Debug.Log("menu");
    }

    public void Credits()
    {
        Debug.Log("credits");
    }

    public void ExitGame()
    {
        Application.Quit();

        Debug.Log("exit"); 
    }
}