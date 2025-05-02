using System;
using System.Collections.Generic;
using Inventory;
using Krisnat;
using Krisnat.Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable CheckNamespace

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    #region Properties
    public bool DamagePopUps { get => damagePopups; private set => damagePopups = value; }
    public bool DashAimingMouse { get => damagePopups; private set => damagePopups = value; }
    public int CurrentLevel { get; set; }
    #endregion

    #region Private Variables
    [SerializeField] private Player player;

    #region Menu UI
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject miniMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject existingSaveButtons;
    #endregion

    #region Settings
    [SerializeField] private TMP_Dropdown resolutionDropdown, qualityDropdown, fpsDropdown;
    [SerializeField] private Toggle fullScreenToggle, damagePopUpsToggle, dashAimingMouseToggle;
    #endregion

    private Resolution[] resolutions;
    private bool damagePopups = true;
    private bool oldGamePaused;
    private static bool newGame;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadGameSettings();
        LoadVideoSettings();

        if (newGame)
        {
            SaveSystem.DeleteAllSaveFiles();
            var save = PlayerSaveData.CreateDefault();
            SaveSystem.SaveData(save);
            CoreClass.GameManager.instance.LoadPlayer(save);
            newGame = false;
        }

        LoadLoadedLevel();

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

        if (SaveSystem.HasLoadFile() && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            existingSaveButtons.SetActive(true);
            playButton.SetActive(false);
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            existingSaveButtons.SetActive(false);
            playButton.SetActive(true);
        }
    }

    void Update()
    {
        if (PlayerPrefs.HasKey("DamagePopUp") && (DamagePopUps ? 1 : 0) != PlayerPrefs.GetInt("DamagePopUp"))
        {
            //DamagePopUps = Convert.ToBoolean(PlayerPrefs.GetInt("DamagePopUp"));
        }

        if (PlayerPrefs.HasKey("DashAimingType") && (DashAimingMouse ? 1 : 0) != PlayerPrefs.GetInt("DashAimingType"))
        {
            DashAimingMouse = Convert.ToBoolean(PlayerPrefs.GetInt("DashAimingType"));
        }

        Screen.fullScreen = fullScreenToggle.isOn;
        if(CoreClass.GameManager.instance){
            if (oldGamePaused != CoreClass.GameManager.instance.GamePaused && CoreClass.GameManager.instance.GamePaused)
            {
                Time.timeScale = 0f;
            }
            else if(oldGamePaused != CoreClass.GameManager.instance.GamePaused)
            {
                Time.timeScale = 1;
            }
            oldGamePaused = CoreClass.GameManager.instance.GamePaused;
        }


        if (
            PlayerInputHandler.Instance
            && PlayerInputHandler.Instance.MenuInput
            && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu"
            && !InventoryManager.Instance.InventoryActiveInHierarchy
            && !InventoryManager.Instance.SpellInventoryActiveInHierarchy
            && !InventoryManager.Instance.CharacterTabActiveInHierarchy
            && !UIManager.Instance.LevelUpInterface.activeInHierarchy
            )
        {
            PlayerInputHandler.Instance.UseMenuInput();

            OpenCloseMenu(!menu.activeInHierarchy);
        }
    }

    public void DeleteSaves()
    {
        SaveSystem.DeleteAllSaveFiles();
    }
    #endregion

    #region Other Methods
    // ReSharper disable once InconsistentNaming
    public void PlayButtonSFX()
    {
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (!button) return;
        AudioManager.instance.PlayButtonSound(0.6f, 0.8f);
    }

    private void LoadLoadedLevel()
    {
        PlayerSaveData data = SaveSystem.LoadPlayer();

        if (data == null)
        {
            CurrentLevel = 1;
            return;
        }

        CurrentLevel = data.currentLevel;
    }

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
            CoreClass.GameManager.instance.GamePaused = true;
            PlayerInputHandler.Instance.StopAllInputs = true;

            AudioManager.instance.PlayMenuSound(1f, 1.2f);
        }
        else
        {
            CloseMenu();
        }
    }
    private void CloseMenu()
    {
        AudioManager.instance.PlayMenuSound(0.7f, 0.9f);
        menu.SetActive(false);
        CoreClass.GameManager.instance.GamePaused = false;
        PlayerInputHandler.Instance.StopAllInputs = false;
        PlayerInputHandler.Instance.StopAttack = false;
    }
    #endregion

    #region Settings
    private void LoadVideoSettings()
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
    private void LoadGameSettings()
    {
        //Damage Popups
        if (PlayerPrefs.HasKey("DamagePopUp"))
        {
            damagePopUpsToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("DamagePopUp"));
            DamagePopUps = damagePopUpsToggle.isOn;
        }
        //Dash Aiming Type
        if (PlayerPrefs.HasKey("DashAimingType"))
        {
            dashAimingMouseToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("DashAimingType"));
            DashAimingMouse = dashAimingMouseToggle.isOn;
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        PlayerPrefs.SetInt("Quality", qualityIndex);
        PlayerPrefs.Save();
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("Resolution", resolutionIndex);
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

    public void SetDashAimingType(bool value)
    {
        DashAimingMouse = value;
        PlayerPrefs.SetInt("DashAimingType", value ? 1 : 0);
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
        if (player != null) SaveSystem.SavePlayer(player);
        //Destroy(player.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void ExitGame()
    {
        if (player != null) SaveSystem.SavePlayer(player);
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void NewGame()
    {
        newGame = true;
    }
    #endregion
} 
