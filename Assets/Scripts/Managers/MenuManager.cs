using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using Photon.Pun;
using Photon.Realtime;

public class MenuManager : MonoBehaviour
{
    public GameObject victoryMenu, loseMenu, mainMenu, pauseMenu, settingsMenu, keybindsMenu, serverListMenu, roomMenu;

    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    public static bool isPaused;

    public static string settedResolution = "";

    public GameObject selectedRoom;

    [SerializeField] private InputActionAsset inputActions;

    private GameObject previousMenu;
    private Resolution[] resolutions;
    private List<Resolution> realResolutions;

    private bool closeDelay;

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChange;
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        HandleGamePause(state == GameState.Pause);
        victoryMenu.SetActive(state == GameState.Vicory);
        loseMenu.SetActive(state == GameState.Lose);
    }

    private void Start()
    {
        HandleMenuVisibility();
        GetResolutionList();
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    private void HandleMenuVisibility()
    {
        if (mainMenu != null)
        {
            mainMenu?.SetActive(true);
        }
        if (pauseMenu != null)
        {
            pauseMenu?.SetActive(false);
        }
        if (serverListMenu != null)
        {
            serverListMenu?.SetActive(false);
        }
        if (roomMenu != null)
        {
            roomMenu?.SetActive(false);
        }
        settingsMenu?.SetActive(false);
        keybindsMenu?.SetActive(false);
    }

    void Update()
    {
        HandlePauseMenuInputs();
        HandleSettingsMenuInputs();
        HandleKeybindingMenuInputs();
        HandleMultiplayerMenuInputs();
    }

    #region General menu options

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Return(GameObject actualMenu)
    {
        previousMenu?.SetActive(true);
        actualMenu?.SetActive(false);
    }

    #endregion

    #region Settings menu options
    public void OpenSettings(GameObject menu)
    {
        previousMenu = menu;
        previousMenu?.SetActive(false);
        settingsMenu?.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = realResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        settedResolution = $"{realResolutions[resolutionIndex].width} x {realResolutions[resolutionIndex].height}";
    }

    private void GetResolutionList()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        realResolutions = new List<Resolution>();
        List<string> options = new List<string>();

        //Crear of resolution duplicates
        string actualResolution = "";
        for (int i = 0; i < resolutions.Length; i++)
        {
            if(!actualResolution.Contains($"{resolutions[i].width} x {resolutions[i].height}"))
            {
                actualResolution = $"{resolutions[i].width} x {resolutions[i].height}";
                realResolutions.Add(resolutions[i]);
            }
        }

        int currentResolutionIndex = 0;
        for (int i = 0; i < realResolutions.Count; i++)
        {
            string option = $"{realResolutions[i].width} x {realResolutions[i].height}";
            options.Add(option);

            if (realResolutions[i].width == Screen.currentResolution.width && realResolutions[i].height == Screen.currentResolution.height && settedResolution.Equals(""))
            {
                currentResolutionIndex = i;
            } else if (settedResolution.Contains($"{realResolutions[i].width} x {realResolutions[i].height}"))
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = currentResolutionIndex;

        resolutionDropdown.RefreshShownValue();
    }

    private void HandleSettingsMenuInputs()
    {
        if (settingsMenu != null && Input.GetButtonDown("Cancel") && settingsMenu.activeSelf)
        {
            Return(settingsMenu);
        }
    }
    #endregion

    #region Main menu options

    public void StartGame()
    {
        isPaused = false;
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion

    #region Pause menu options

    private void HandleGamePause(bool paused)
    {
        if(paused)
        {
            StartCoroutine(CloseMenuDelay());
        }
        if (pauseMenu != null)
        {
            pauseMenu?.SetActive(paused);
        }
    }

    public void ResumeGame()
    {
        GameManager.Instance.UpdateGameState(GameState.Combat);
    }

    private void HandlePauseMenuInputs()
    {
        if(pauseMenu != null && pauseMenu.activeSelf && Input.GetButtonDown("Cancel") && !closeDelay) 
        {
            GameManager.Instance.UpdateGameState(GameState.Combat);
        }
    }

    IEnumerator CloseMenuDelay()
    {
        closeDelay = true;
        yield return new WaitForEndOfFrame();
        closeDelay = false;

    }

    #endregion

    #region Keybinds menu options

    public void OpenKeybinds()
    {
        settingsMenu?.SetActive(false);
        keybindsMenu?.SetActive(true);
    }

    public void KeybindsReturn()
    {
        settingsMenu?.SetActive(true);
        keybindsMenu?.SetActive(false);
    }

    public void ResetAllBindings()
    {
        foreach(InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }

    private void HandleKeybindingMenuInputs()
    {
        if (keybindsMenu != null && Input.GetButtonDown("Cancel") && keybindsMenu.activeSelf)
        {
            KeybindsReturn();
        }
    }

    #endregion

    #region Server menu options

    public void OpenServerList()
    {
        mainMenu?.SetActive(false);
        serverListMenu?.SetActive(true);
        previousMenu = mainMenu;
        DestroyLoadedServers();
        LoadServers();
    }

    public void ChangeOrGenerateNickName(TMP_InputField inputName)
    {
        if (!NetworkManager.Instance.IsConnected())
        {
            return;
        }

        string name = inputName.text;

        if (name == "")
        {
            name = "Player";
        }

        NetworkManager.Instance.player.NickName = name;
    }

    public void ConnectToServer()
    {
        if (!NetworkManager.Instance.IsConnected())
        {
            return;
        }

        if (selectedRoom != null)
        {
            NetworkManager.Instance.JoinRoom(selectedRoom.GetComponent<ServerScript>().roomNameTMP.text);
        }
    }

    public void SetSelectedRoom(GameObject newSelectedRoom)
    {
        if (selectedRoom != null)
        {
            selectedRoom.GetComponent<ServerScript>().IsSelected(false);
        }
        selectedRoom = newSelectedRoom;
        selectedRoom.GetComponent<ServerScript>().IsSelected(true);
    }

    public void CreateServer(TMP_InputField inputName)
    {
        if (!NetworkManager.Instance.IsConnected())
        {
            return;
        }

        string name = inputName.text;

        if (name == "")
        {
            name = $"{NetworkManager.Instance.player.NickName}'s room";
        }

        int serverWithThisName = 0;

        foreach(RoomInfo roomInfo in NetworkManager.Instance.GetServerList())
        {
            if(roomInfo.Name == name)
            {
                serverWithThisName++;
            }
        }

        if(serverWithThisName > 0)
        {
            name = $"{name} {serverWithThisName}";
        }

        NetworkManager.Instance.CreateRoom(name);
    }

    public void LoadServers()
    {
        if (!NetworkManager.Instance.IsConnected())
        {
            return;
        }

        if (!NetworkManager.Instance.IsInLobby())
        {
            NetworkManager.Instance.JoinLobby();
        }
        StartCoroutine(ServerLoad());
    }

    public void UnselectServer()
    {
        selectedRoom = null;
    }

    IEnumerator ServerLoad()
    {
        yield return new WaitForEndOfFrame();

        if (NetworkManager.Instance.IsInLobby())
        {
            //Get items
            Transform servers = GameObject.Find("Servers").transform;
            GameObject serverPanel = (GameObject)Resources.Load("Prefabs/UI/ListItems/ListedServer");

            string previousName = "";
            //Load server panels
            for (int i = 0; i < NetworkManager.Instance.GetServerList().Count; i++)
            {
                if(previousName == NetworkManager.Instance.GetServerList()[i].Name)
                {
                    continue;
                }

                //Set panel position
                float yPosition = servers.localPosition.y - serverPanel.transform.GetComponent<RectTransform>().sizeDelta.y * i - serverPanel.transform.GetComponent<RectTransform>().sizeDelta.y / 2;
                GameObject actualServerPanel = Instantiate(serverPanel, new Vector3(0, 0, 0), Quaternion.identity, servers);
                actualServerPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, yPosition, 0);

                //Fill panel info
                RoomInfo roomInfo = NetworkManager.Instance.GetServerList()[i];
                actualServerPanel.GetComponent<ServerScript>().Inicialize(roomInfo.Name, roomInfo.PlayerCount.ToString(), roomInfo.MaxPlayers.ToString());
                previousName = roomInfo.Name;
            }

            //Resize container
            servers.GetComponent<RectTransform>().sizeDelta = new Vector2(servers.GetComponent<RectTransform>().sizeDelta.x, serverPanel.transform.GetComponent<RectTransform>().sizeDelta.y * NetworkManager.Instance.GetServerList().Count);
        }
        else
        {
            StartCoroutine(ServerLoad());
        }
    }

    public void DestroyLoadedServers()
    {
        //Get servers item
        Transform servers = GameObject.Find("Servers").transform;

        //Destroy server panels
        for (int i = 0; i < servers.childCount; i++)
        {
            Destroy(servers.GetChild(i).gameObject);
        }

        //Resize container
        servers.GetComponent<RectTransform>().sizeDelta = new Vector2 (servers.GetComponent<RectTransform>().sizeDelta.x, 0);
    }

    public void LeaveFromLobby()
    {
        NetworkManager.Instance.DisconnectFromLobby();
    }

    private void HandleMultiplayerMenuInputs()
    {
        if (serverListMenu != null && Input.GetButtonDown("Cancel") && serverListMenu.activeSelf)
        {
            Return(serverListMenu);
        }
    }

    #endregion

    #region Room menu options

    public void OpenRoom()
    {
        serverListMenu?.SetActive(false);
        roomMenu?.SetActive(true);
        previousMenu = serverListMenu;

        roomMenu.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = NetworkManager.Instance.GetRoomName();
        roomMenu.transform.Find("StartButton").gameObject.SetActive(NetworkManager.Instance.IsMaster());
        LoadPlayers();
    }

    public void LoadPlayers()
    {
        DestroyLoadedPlayers();

        //Get items
        Transform players = GameObject.Find("Players").transform;
        GameObject playerPanel = (GameObject)Resources.Load("Prefabs/UI/ListItems/ListedPlayer");

        //Load server panels
        for (int i = 0; i < NetworkManager.Instance.GetRoomPlayers().Length; i++)
        {
            //Set panel position
            float yPosition = players.localPosition.y - playerPanel.transform.GetComponent<RectTransform>().sizeDelta.y * i - playerPanel.transform.GetComponent<RectTransform>().sizeDelta.y / 2;
            GameObject actualPlayerPanel = Instantiate(playerPanel, new Vector3(0, 0, 0), Quaternion.identity, players);
            actualPlayerPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, yPosition, 0);

            //Fill panel info
            Photon.Realtime.Player player = NetworkManager.Instance.GetRoomPlayers()[i];
            actualPlayerPanel.GetComponent<ListPlayerScript>().Inicialize(player.NickName, player.IsMasterClient);
        }

        //Resize container
        players.GetComponent<RectTransform>().sizeDelta = new Vector2(players.GetComponent<RectTransform>().sizeDelta.x, playerPanel.transform.GetComponent<RectTransform>().sizeDelta.y * NetworkManager.Instance.GetRoomPlayers().Length);
    }

    public void DisconnectFromServer()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.IsInRoom())
        {
            NetworkManager.Instance.DisconnectFromRoom();
        }
    }

    public void StartMultiplayerGame()
    {
        NetworkManager.Instance.LoadLevel(1);
    }

    public void DestroyLoadedPlayers()
    {
        //Get servers item
        Transform players = GameObject.Find("Players").transform;

        //Destroy server panels
        for (int i = 0; i < players.childCount; i++)
        {
            Destroy(players.GetChild(i).gameObject);
        }

        //Resize container
        players.GetComponent<RectTransform>().sizeDelta = new Vector2(players.GetComponent<RectTransform>().sizeDelta.x, 0);
    }

    public void RoomReturn(GameObject actualMenu)
    {
        previousMenu?.SetActive(true);
        actualMenu?.SetActive(false);
        previousMenu = mainMenu;
    }
    #endregion

}
