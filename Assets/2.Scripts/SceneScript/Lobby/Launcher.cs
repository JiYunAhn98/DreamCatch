using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using DefineHelper;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region [ 변수 ]
    // Scene 대체 패널 모음
    [Header("Panels")]
    [SerializeField] BGPanel _bgPanel;
    [SerializeField] LoginPanel _loginPanel;
    [SerializeField] LobbyPanel _lobbyPanel;
    [SerializeField] RoomListPanel _roomListPanel;
    [SerializeField] InRoomPanel _inRoomPanel;

    [SerializeField] ObjectSpawn _myChar;

    // 정보 변수
    string gameVersion = "1";   // 게임 버젼이 같은 서버끼리 만나도록 한다.
    BasePanel _nowPanel;
    List<RoomInfo> _myList = new List<RoomInfo>();

    #endregion [ 변수 ]

    #region [ MonoBehaviour Callbacks ]
    void Awake()
    {
        PhotonNetwork.GameVersion = this.gameVersion;

        _loginPanel.gameObject.SetActive(false);
        _lobbyPanel.gameObject.SetActive(false);
        _roomListPanel.gameObject.SetActive(false);
        _inRoomPanel.gameObject.SetActive(false);

        _bgPanel.MyTurnInit();

        // 가장 처음에 들어왔을 때, 연결이 끊겼을 때 => Login
        if (PhotonNetwork.NetworkClientState == ClientState.PeerCreated || PhotonNetwork.NetworkClientState == ClientState.Disconnected)
        {
            SettingManager._instance.SettingStart();
            UIManager._instance.PopupLoad();
            NowPanelSet(_loginPanel);
        }
        // 현재 방에 있을 때, Dream Catch화면에서 나옴 => InRoom
        else if (PhotonNetwork.NetworkClientState == ClientState.Joined)
        {
            _myChar.InstanceCharacter(PhotonNetwork.LocalPlayer.CustomProperties[ePlayerProperty.Character.ToString()].ToString());
            NowPanelSet(_inRoomPanel);
        }
        // 특수한 경우가 아니면 다 => Lobby 
        else
        {
            NowPanelSet(_lobbyPanel);
        }
    }
    #endregion [ MonoBehaviour Callbacks ]

    #region [ Photon Callbacks ]
    public override void OnDisconnected(DisconnectCause cause)
    {
        _bgPanel.MyTurnInit();
        NowPanelSet(_loginPanel);
        UIManager._instance.OpenAlarm(false, PhotonNetwork.NetworkClientState.ToString() + "\n연결끊김\n이유: " + cause);
    }
    public override void OnConnectedToMaster()
    {
        _bgPanel.MyTurnInit();
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(ePlayerProperty.Character.ToString()))
        {
            // 내가 껐을 때 사용했던 캐릭터를 여기서 불러옴
            PhotonNetwork.LocalPlayer.CustomProperties.Add(ePlayerProperty.Character.ToString(), eCharacter.RPGWarrior.ToString());
            _myChar.InstanceCharacter(PhotonNetwork.LocalPlayer.CustomProperties[ePlayerProperty.Character.ToString()].ToString());
        }
         if (_nowPanel == _loginPanel)
            NowPanelSet(_lobbyPanel);
        else
            PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()        // 마스터 서버에서 로비로 들어갈 때
    {
        _myList.Clear();
        NowPanelSet(_roomListPanel);
    }
    public override void OnLeftLobby()
    {
        NowPanelSet(_lobbyPanel);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 그냥 바로 넣으면 Error가 나기도 한다.
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!_myList.Contains(roomList[i])) _myList.Add(roomList[i]);
                else _myList[_myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (_myList.IndexOf(roomList[i]) != -1) _myList.RemoveAt(_myList.IndexOf(roomList[i]));
        }
        _roomListPanel.SyncRoomList(_myList);
    }
    public override void OnCreatedRoom()
    {
        NowPanelSet(_inRoomPanel);
    }
    public override void OnJoinedRoom()
    {
        NowPanelSet(_inRoomPanel);
        _inRoomPanel.CallRoomUpdate("");
    }
    public override void OnLeftRoom()
    {
        // 씬 부르기
        PhotonNetwork.JoinLobby();
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        _inRoomPanel.CallRoomUpdate();
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        _inRoomPanel.CallRoomUpdate(newPlayer.NickName + "님이 입장하였습니다.");
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        _inRoomPanel.CallRoomUpdate(otherPlayer.NickName + "님이 퇴장하였습니다.");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        UIManager._instance.OpenAlarm(false, returnCode + ": " + message + "\n입장에 실패했습니다.");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        SettingManager._instance.RoomOptionSetting(8, "RandomRoom", eGameMode.Survival.ToString());
        PhotonNetwork.CreateRoom(System.DateTime.Now.ToString() + "_" + PhotonNetwork.NickName, SettingManager._instance._roomOptions);
        UIManager._instance.OpenAlarm(false, returnCode + ": " + message + "\n입장에 실패해 방을 새로 만듭니다.");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        UIManager._instance.OpenAlarm(false, PhotonNetwork.NetworkClientState.ToString() + " / 방생성 실패 / 이유: " + message);
    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        _inRoomPanel.CallRoomUpdate(newMasterClient.NickName + "님이 방장이 되었습니다.");
    }
    #endregion [ Photon Callbacks ]

    #region [ 내부 함수 ]
    void NowPanelSet(BasePanel now)
    {
        if (_nowPanel == now) return;

        if (_nowPanel != null) _nowPanel.gameObject.SetActive(false);
        _nowPanel = now;
        _nowPanel.MyTurnInit();
        UIManager._instance.CloseAllPopup();
    }
    //void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();
    //    style.normal.textColor = Color.black;
    //    style.fontSize = 50;
    //    GUI.Label(new Rect(Screen.width / 2 - 500, 10, 500, 50), PhotonNetwork.NetworkClientState.ToString(), style);
    //}
    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, Screen.height / 2, 100, 100), "CharChange"))
    //    {
    //        eCharacter nowPick = (eCharacter)System.Enum.Parse(typeof(eCharacter), PhotonNetwork.LocalPlayer.CustomProperties[ePlayerProperty.Character.ToString()].ToString());
    //        if (nowPick == eCharacter.RPGWarrior)
    //        {
    //            _myChar.InstanceCharacter(eCharacter.SFSoldier);
    //            PhotonNetwork.LocalPlayer.CustomProperties[ePlayerProperty.Character.ToString()] = eCharacter.SFSoldier.ToString();
    //        }
    //        else
    //        {
    //            _myChar.InstanceCharacter(eCharacter.RPGWarrior);
    //            PhotonNetwork.LocalPlayer.CustomProperties[ePlayerProperty.Character.ToString()] = eCharacter.RPGWarrior.ToString();
    //        }
    //    }
    //}
    #endregion [ 내부 함수 ]
}
