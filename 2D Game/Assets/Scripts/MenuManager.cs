using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using Core;
using Krisnat.Assets.Scripts;
using Krisnat;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    #region Private Variables
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject miniMenu;
    [SerializeField] private GameObject settings;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown fpsDropdown;

    [SerializeField] private float openingMenuDuration = 0.2f;
    [SerializeField] private float closingMenuDuration = 0.2f;

    private Resolution[] resolutions;
    #endregion

    #region Unity Methods
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

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionsList);

        if (!PlayerPrefs.HasKey("Resolution"))
        {
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        LoadVideoSettings();
    }

    void Update()
    {
        if (PlayerInputHandler.Instance.MenuInput && SceneManager.GetActiveScene().name != "MainMenu")
        {
            PlayerInputHandler.Instance.UseMenuInpit();
            if (menu.activeSelf)
            {
                Unpause();
                menu.SetActive(false);
                //UIManager.Instance.OpenUIAnimation(miniMenu, 0.05f, closingMenuDuration, false);
            }
            else
            {
                menu.SetActive(true);
                /*var scale = miniMenu.transform.localScale.x;
                miniMenu.transform.localScale = new Vector3(0.05f, 0.05f, miniMenu.transform.localScale.z);
                UIManager.Instance.OpenUIAnimation(miniMenu, scale, openingMenuDuration, true);

                Core.GameManager.Instance.gamePaused = true;*/
                PauseGame();
            }
        }
    }
    #endregion

    #region Other Methods
    private void PauseGame()
    {
        if (Core.GameManager.Instance.gamePaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    public void Unpause()
    {
        Core.GameManager.Instance.gamePaused = false;
        PauseGame();
    }

    public void LoadVideoSettings()
    {
        if (PlayerPrefs.HasKey("Quality"))
        {
            qualityDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("Quality"));
        }

        if (PlayerPrefs.HasKey("Resolution"))
        {
            resolutionDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("Resolution"));
        }

        if (PlayerPrefs.HasKey("Fps"))
        {
            fpsDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("Fps"));
        }
    }
    #endregion

    #region Settings
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        PlayerPrefs.SetInt("Quality", qualityIndex);
        PlayerPrefs.Save();
    }
    public void SetResolution(int resoulutionIndex)
    {
        Resolution resolution = resolutions[resoulutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("Resolution", resoulutionIndex);
        PlayerPrefs.Save();
    }
    public void SetMaxFramerate(int fpsIndex)
    {
        QualitySettings.vSyncCount = 0;

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
                Application.targetFrameRate = 165;
                break;
            case 6:
                Application.targetFrameRate = int.MaxValue;
                break;
        }

        PlayerPrefs.SetInt("Fps", fpsIndex);
        PlayerPrefs.Save();
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    #endregion

    #region Menu
    public void StartSettings()
    {
        settings.SetActive(true);
        miniMenu.SetActive(false);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ExitGame()
    {
        var player = transform.Find("Player").GetComponent<Player>();
        SaveSystem.SavePlayer(player);
        Application.Quit();

        Debug.Log("exit");
    }
    public void Back()
    {
        settings.SetActive(false);
        menu.SetActive(true);
        miniMenu.SetActive(true);
    }
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    #endregion
}