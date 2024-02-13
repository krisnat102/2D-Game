using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using Krisnat.Assets.Scripts;
using Krisnat;
using Inventory;

public class MenuManager : MonoBehaviour
{
    #region Private Variables
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject miniMenu;
    [SerializeField] private GameObject settings;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown fpsDropdown;

    [SerializeField] private float closingMenuDuration = 0.2f;

    private Resolution[] resolutions;
    private Animator animator;
    private float scale;
    #endregion

    #region Unity Methods
    private void Start()
    {
        animator = menu.GetComponentInChildren<Animator>();

        resolutions = Screen.resolutions;

        scale = miniMenu.transform.localScale.x;

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
        if (Core.GameManager.Instance.GamePaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1;
        }

        if (
            PlayerInputHandler.Instance.MenuInput && SceneManager.GetActiveScene().name != "MainMenu"
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
            //UIManager.Instance.OpenCloseUI(miniMenu, scale, openingMenuDuration, false, true, true);
            //animator.SetTrigger("menuOpen");
            menu.SetActive(true);
            Core.GameManager.Instance.GamePaused = true;
            PlayerInputHandler.Instance.StopAllInputs = true;
        }
        else
        {
            //UIManager.Instance.OpenCloseUI(miniMenu, scale, closingMenuDuration, false, true, false);
            //Invoke("CloseMenu", closingMenuDuration);
            //animator.SetTrigger("menuClose");
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

    #region Menu Buttons
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