using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Krisnat.Assets.Scripts;
using Krisnat;
using Inventory;
using System;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public bool DamagePopUps { get => damagePopups; private set => damagePopups = value; }
    public bool DashAimingMouse { get => damagePopups; private set => damagePopups = value; }
    public int CurrentLevel { get; set; }

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
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
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
            CoreClass.GameManager.Instance.LoadPlayer(save);
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

        Screen.fullScreen = fullScreenToggle.isOn;
        if(CoreClass.GameManager.Instance != null){
            if (oldGamePaused != CoreClass.GameManager.Instance.GamePaused && CoreClass.GameManager.Instance.GamePaused)
            {
                Time.timeScale = 0f;
            }
            else if(oldGamePaused != CoreClass.GameManager.Instance.GamePaused)
            {
                Time.timeScale = 1;
            }
            oldGamePaused = CoreClass.GameManager.Instance.GamePaused;
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
    public void PlayButtonSFX()
    {
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (!button) return;
        AudioManager.Instance.PlayButtonSound(0.6f, 0.8f);
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
            CoreClass.GameManager.Instance.GamePaused = true;
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
        CoreClass.GameManager.Instance.GamePaused = false;
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