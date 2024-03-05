using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using Krisnat.Assets.Scripts;
using Krisnat;
using Inventory;
using System;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public bool DamagePopUps { get; private set; }

    #region Private Variables
    [SerializeField] private Player player;

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject miniMenu;
    [SerializeField] private GameObject settings;

    [SerializeField] private TMP_Dropdown resolutionDropdown, qualityDropdown, fpsDropdown;
    [SerializeField] private Toggle fullScreenToggle, damagePopUpsToggle;

    private Resolution[] resolutions;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
    }

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
        LoadGameSettings();
    }

    void Update()
    {
        Screen.fullScreen = fullScreenToggle.isOn;

        if (Core.GameManager.Instance != null && Core.GameManager.Instance.GamePaused)
        {
            Time.timeScale = 0f;
        }
        else if (Core.GameManager.Instance != null)
        {
            Time.timeScale = 1;
        }

        if (
            PlayerInputHandler.Instance != null 
            && PlayerInputHandler.Instance.MenuInput
            && SceneManager.GetActiveScene().name != "MainMenu"
            && !InventoryManager.Instance.InventoryActiveInHierarchy
            && !InventoryManager.Instance.SpellInventoryActiveInHierarchy
            && !InventoryManager.Instance.CharacterTabActiveInHierarchy
            && !UIManager.Instance.LevelUpInterface.activeInHierarchy
            )
        {
            PlayerInputHandler.Instance.UseMenuInpit();

            if (!menu.activeInHierarchy)
            {
                OpenCloseMenu(true);
            }
            else
            {
                OpenCloseMenu(false);
            }
        }
    }
    #endregion

    #region Other Methods
    public void OpenCloseMenu(bool openOrClose)
    {
        if (openOrClose)
        {
            menu.SetActive(true);

            if (settings.activeInHierarchy)
            {
                settings.SetActive(false);
                miniMenu.SetActive(true);
            }
            Core.GameManager.Instance.GamePaused = true;
            PlayerInputHandler.Instance.StopAllInputs = true;
        }
        else
        {
            CloseMenu();
        }
    }
    private void CloseMenu()
    { 
        menu.SetActive(false);
        Core.GameManager.Instance.GamePaused = false;
        PlayerInputHandler.Instance.StopAllInputs = false;
        PlayerInputHandler.Instance.StopAttack = false;
    }
    #endregion

    #region Settings
    public void LoadVideoSettings()
    {
        //Quality
        if (PlayerPrefs.HasKey("Quality"))
        {
            qualityDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("Quality"));
        }
        //Resolution
        if (PlayerPrefs.HasKey("Resolution"))
        {
            resolutionDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("Resolution"));
        }
        //FPS
        if (PlayerPrefs.HasKey("Fps"))
        {
            fpsDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("Fps"));
        }
        //FullScreen
        if (PlayerPrefs.HasKey("FullScreen"))
        {
            fullScreenToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreen"));
        }
    }
    public void LoadGameSettings()
    {
        //Damage Pop Ups
        if (PlayerPrefs.HasKey("DamagePopUp"))
        {
            damagePopUpsToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("DamagePopUp"));
            DamagePopUps = damagePopUpsToggle.isOn;
        }
    }

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
        PlayerPrefs.SetInt("FullScreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetDamagePopUps(bool value)
    {
        DamagePopUps = value;
        PlayerPrefs.SetInt("DamagePopUp", value ? 1 : 0);
        PlayerPrefs.Save();
    }
    #endregion

    #region Menu Buttons
    public void StartSettings()
    {
        settings.SetActive(true);
        miniMenu.SetActive(false);
    }
    public void MainMenu()
    {
        SaveSystem.SavePlayer(player);
        Destroy(player.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
    public void ExitGame()
    {
        if(player != null) SaveSystem.SavePlayer(player);
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