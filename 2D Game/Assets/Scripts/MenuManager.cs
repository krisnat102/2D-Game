using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject Menu;

    public GameObject Settings;

    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> resolutionsList = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolution = resolutions[i].width + " x " + resolutions[i].height;

            resolutionsList.Add(resolution);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionsList);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

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

    public void SetQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetResolution (int resoulutionIndex)
    {
        Resolution resolution = resolutions[resoulutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetMaxFramerate(int fpsIndex)
    {
        QualitySettings.vSyncCount = 0;

        int targetFrameRate;

        switch (fpsIndex)
        {
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Application.targetFrameRate = 75;
                break;
            case 3:
                Application.targetFrameRate = 120;
                break;
            case 4:
                Application.targetFrameRate = 144;
                break;
            case 5:
                Application.targetFrameRate = 169;
                break;
            case 6:
                Application.targetFrameRate = int.MaxValue;
                break;
        }
    }
    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
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