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
    [SerializeField] Text stateText;
    [SerializeField] Text roomNameText;
    [SerializeField] Text winText;
    [SerializeField] Text loseText;
    [SerializeField] GameObject waitingPanel;
    [SerializeField] Text playerCountText;
    [SerializeField] Text countdownText;
    [SerializeField] Text waitingText;
    [SerializeField] Transform[] spawnPoints;

    bool isLeftTeam;

    public MoveJoystick moveJoystick;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 240;
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
    }

    void Start()
    {
        nicknameInput.text = "User" + Random.Range(0, 1000);
        roomNameInput.text = "Room" + Random.Range(0, 1000);

        startButton.interactable = false;
        fastStartButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
        stateText.text = "서버 접속 시도중....";
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
        stateText.text = "서버 접속 완료";
        startButton.interactable = true;
        fastStartButton.interactable = true;
    }

    public override void OnCreatedRoom()
    {
        isLeftTeam = true;
    }

    public override void OnJoinedRoom()
    {
        SetActivePanel(inGamePanel.name);
        waitingPanel.SetActive(true);
        roomNameText.text = "방 이름 : " + PhotonNetwork.CurrentRoom.Name;

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
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions, null);
    }

    public void GameStartBtn()
    {
        if (!string.IsNullOrWhiteSpace(nicknameInput.text) && !string.IsNullOrWhiteSpace(roomNameInput.text))
        {
            PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, roomOptions, null);
        }
        else
        {
            stateText.text = "닉네임 혹은 방 이름이 비어있습니다.";
        }
    }

    public void FastGameStartBtn()
    {
        if (!string.IsNullOrWhiteSpace(nicknameInput.text))
        {
            PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            stateText.text = "방 이름이 비어있습니다.";
        }
    }

    public void RestartGame()
    {
        nicknameInput.text = "";
        PhotonNetwork.LeaveRoom();
        Init();
        SetActivePanel(lobbyPanel.name);
        nicknameInput.text = "User" + Random.Range(0, 1000);

        moveJoystick.blinkButton.interactable = true;
        moveJoystick.coolTimeText.gameObject.SetActive(false);
        moveJoystick.coolTimeImage.fillAmount = 1f;
    }

    public void QuitBtn(int num)
    {
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
    }

    IEnumerator GameStart()
    {
        waitingText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(true);

        int count = 3;

        while (count > 0)
        {
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
        StartCoroutine(GameStart());
    }

    [PunRPC]
    void PlayerCountT()
    {
        playerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
}
