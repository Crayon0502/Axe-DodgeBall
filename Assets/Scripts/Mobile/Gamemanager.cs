using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Gamemanager : MonoBehaviourPunCallbacks
{
    public static Gamemanager instance;

    bool isWin;

    public bool IsWin {
        get { return isWin; }
        set { 
            isWin = value;
            GameEnd(); 
        } }

    public bool isGameStart;
    public PhotonView pv;
    [SerializeField] GameObject inGamePanel;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject winLosePanel;
    [SerializeField] GameObject quitPanel;
    [SerializeField] InputField nicknameInput;
    [SerializeField] InputField roomNameInput;
    [SerializeField] Button startButton;
    [SerializeField] Button fastStartButton;
    [SerializeField] Button exitButton;
    [SerializeField] Text stateText;
    [SerializeField] Text virsionText;
    [SerializeField] Text roomNameText;
    [SerializeField] Text winText;
    [SerializeField] Text loseText;
    [SerializeField] GameObject waitingPanel;
    [SerializeField] Text playerCountText;
    [SerializeField] Text countdownText;
    [SerializeField] Text waitingText;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform cam;
    [SerializeField] AudioSource countAudioSource;

    AudioSource audioSource;

    bool isLeftTeam;

    public MoveJoystick moveJoystick;
    string gameVersion = "0.1.0";

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 240;
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.GameVersion = gameVersion;
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        nicknameInput.text = "User" + Random.Range(0, 1000);
        roomNameInput.text = "Room" + Random.Range(0, 1000);

        startButton.interactable = false;
        fastStartButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
        stateText.text = "���� ���� �õ���....";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    quitPanel.SetActive(true);
                }
            }
        }
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        stateText.text = "���� ���� �Ϸ�";
        virsionText.text = gameVersion;
        startButton.interactable = true;
        fastStartButton.interactable = true;
    }

    public override void OnCreatedRoom()
    {
        isLeftTeam = true;
    }

    public override void OnJoinedRoom()
    {
        if (isGameStart)
            PhotonNetwork.LeaveRoom();

        SetActivePanel(inGamePanel.name);
        waitingPanel.SetActive(true);
        roomNameText.text = "�� �̸� : " + PhotonNetwork.CurrentRoom.Name;

        if (isLeftTeam)
            PhotonNetwork.Instantiate("Player", spawnPoints[0].position, Quaternion.identity);
        else
            PhotonNetwork.Instantiate("Player", spawnPoints[1].position, Quaternion.identity);

        //StartCoroutine(DestroyObj());
        CheckPlayerCount();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom("Room" + Random.Range(0, 1000), roomOptions, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        stateText.text = "������ �� ���� ���Դϴ�. �ٸ� �濡 �������ֽʽÿ�";
    }

    public void GameStartBtn()
    {
        cam.GetComponent<CameraMove>().enabled = true;
        audioSource.Play();

        if (!string.IsNullOrWhiteSpace(nicknameInput.text) && !string.IsNullOrWhiteSpace(roomNameInput.text))
        {
            PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, roomOptions, null);
        }
        else
        {
            stateText.text = "�г��� Ȥ�� �� �̸��� ����ֽ��ϴ�.";
        }
    }

    public void FastGameStartBtn()
    {
        cam.GetComponent<CameraMove>().enabled = true;
        audioSource.Play();

        if (!string.IsNullOrWhiteSpace(nicknameInput.text))
        {
            PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            stateText.text = "�� �̸��� ����ֽ��ϴ�.";
        }
    }

    public void RestartGame()
    {
        audioSource.Play();
        string tempName = nicknameInput.text;
        nicknameInput.text = "";
        PhotonNetwork.LeaveRoom();
        Init();
        SetActivePanel(lobbyPanel.name);
        nicknameInput.text = tempName;

        moveJoystick.blinkButton.interactable = true;
        moveJoystick.coolTimeText.gameObject.SetActive(false);
        moveJoystick.coolTimeImage.fillAmount = 1f;
        stateText.text = "���� ���� �Ϸ�";
    }

    public void QuitBtn(int num)
    {
        audioSource.Play();

        if (num == 1)
        {
            Application.Quit();
        }
        else
        {
            quitPanel.SetActive(false);
        }
    }

    void Init()
    {
        cam.GetComponent<CameraMove>().enabled = false;
        cam.transform.position = new Vector3(0, 12.5f, -10);
        waitingPanel.SetActive(true);
        waitingText.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(false);
        winLosePanel.SetActive(false);
        isWin = false;
        isLeftTeam = false;
        isGameStart = false;
    }

    void CheckPlayerCount()
    {
        pv.RPC("PlayerCountT", RpcTarget.AllBuffered);

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            pv.RPC("StartGame", RpcTarget.AllBuffered);
    }

    void SetActivePanel(string panelName)
    {
        inGamePanel.SetActive(panelName.Equals(inGamePanel.name));
        lobbyPanel.SetActive(panelName.Equals(lobbyPanel.name));
    }

    void GameEnd()
    {
        winLosePanel.SetActive(true);
        winText.gameObject.SetActive(IsWin);
        loseText.gameObject.SetActive(!IsWin);
        exitButton.gameObject.SetActive(true);
    }

    IEnumerator GameStart()
    {
        exitButton.gameObject.SetActive(false);
        waitingText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(true);

        int count = 3;

        while (count > 0)
        {
            countAudioSource.Play();
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1);
            count--;
        }

        waitingPanel.SetActive(false);
        isGameStart = true;
    }

    [PunRPC]
    void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        StartCoroutine(GameStart());
    }

    [PunRPC]
    void PlayerCountT()
    {
        playerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
}
