using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;
using Inventory;
using Krisnat;
using Krisnat.Assets.Scripts;

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
    public static bool newGame = false;
    
    private string SettingsFile => Path.Combine(Application.persistentDataPath, "settings.json");
    
    private SettingsData settingsData = new SettingsData();
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
            DamagePopUps = Convert.ToBoolean(PlayerPrefs.GetInt("DamagePopUp"));
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
            PlayerInputHandler.Instance != null
            && PlayerInputHandler.Instance.MenuInput
            && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu"
            && !InventoryManager.Instance.InventoryActiveInHierarchy
            && !InventoryManager.Instance.SpellInventoryActiveInHierarchy
            && !InventoryManager.Instance.CharacterTabActiveInHierarchy
            && !UIManager.Instance.LevelUpInterface.activeInHierarchy
            )
        {
            PlayerInputHandler.Instance.UseMenuInput();

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
    
    #region Settings Saving System
    public void LoadVideoSettings()
    {
        if (File.Exists(SettingsFile))
        {
            string json = File.ReadAllText(SettingsFile);
            settingsData = JsonUtility.FromJson<SettingsData>(json);
    
            qualityDropdown.SetValueWithoutNotify(settingsData.qualityIndex);
            resolutionDropdown.SetValueWithoutNotify(settingsData.resolutionIndex);
            fpsDropdown.SetValueWithoutNotify(settingsData.fpsIndex);
            fullScreenToggle.isOn = settingsData.fullscreen;
        }
        else
        {
            //Creates a new file if no file exists
            settingsData = new SettingsData();
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
        //Dash Aiming Type
        if (PlayerPrefs.HasKey("DashAimingType"))
        {
            dashAimingMouseToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("DashAimingType"));
            DashAimingMouse = dashAimingMouseToggle.isOn;
        }
        
        if (File.Exists(SettingsFile))
        {
            string json = File.ReadAllText(SettingsFile);
            settingsData = JsonUtility.FromJson<SettingsData>(json);

            damagePopUpsToggle.isOn = settingsData.damagePopUps;
            DamagePopUps =settingsData.damagePopUps;
            
            dashAimingMouseToggle.isOn = settingsData.dashAimingMouse;
            DashAimingMouse = settingsData.dashAimingMouse;
        }
        else
        {
            //Creates a new file if no file exists
            settingsData = new SettingsData();
        }
    }
    
    private void SaveVideoSettings()
    {
        settingsData.qualityIndex = qualityDropdown.value;
        settingsData.resolutionIndex = resolutionDropdown.value;
        settingsData.fpsIndex = fpsDropdown.value;
        settingsData.fullscreen = fullScreenToggle.isOn;

        string json = JsonUtility.ToJson(settingsData, true);
        File.WriteAllText(SettingsFile, json);
    }
    #endregion

    #region Settings Setters
    private void SaveGameSettings()
    {
        settingsData.damagePopUps = damagePopUpsToggle.isOn;
        settingsData.dashAimingMouse = dashAimingMouseToggle.isOn;

        string json = JsonUtility.ToJson(settingsData, true);
        File.WriteAllText(SettingsFile, json);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        SaveVideoSettings();
    }
    public void SetResolution(int resoulutionIndex)
    {
        Resolution resolution = resolutions[resoulutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        SaveVideoSettings();
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

        SaveVideoSettings();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        
        SaveVideoSettings();
    }

    public void SetDamagePopUps(bool value)
    {
        DamagePopUps = value;
        
        SaveGameSettings();
    }

    public void SetDashAimingType(bool value)
    {
        DashAimingMouse = value;
        
        SaveGameSettings();
    }
    #endregion
    
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

