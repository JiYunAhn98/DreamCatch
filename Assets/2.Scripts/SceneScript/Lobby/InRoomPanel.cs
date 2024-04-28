using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using DefineHelper;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// JoinedRoom 상태
/// </summary>
public class InRoomPanel : BasePanel
{
    [Header("RoomInfo Section")]
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] TextMeshProUGUI _roomNum;
    [SerializeField] TextMeshProUGUI _mode;
    [SerializeField] Button _settingRoomBtn;

    [Header("Btn Section")]
    [SerializeField] Button _readyBtn;
    [SerializeField] Button _startBtn;
    [SerializeField] Button _settingCharBtn;
    [SerializeField] Button _exitBtn;

    [Header("Character Section")]
    [SerializeField] CharacterInfoSlot[] _slots;   // 캐릭터 변경시 캐릭터를 바꿔주고 입장 시 닉네임을 띄워준다. Ready의 상태를 상호 교환한다.

    [Header("Chatting Section")]
    [SerializeField] TMP_InputField _chatInput;
    [SerializeField] GameObject _content;
    [SerializeField] GameObject _prefabChatLine;

    PhotonView _pv;
    int _index;

    public override void MyTurnInit()
    {
        gameObject.SetActive(true);
        ChatClear();
        _index = -1;

        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
        _pv = GetComponent<PhotonView>();
        
        CallRoomUpdate();
    }

    #region [ GameBtn Event ]
    public void SettingRoomBtn()
    {
        // 현재 방의 설정을 담은 SettingPopup을 띄운다.
        UIManager._instance.OpenPopup(ePopup.RoomSettingWnd);
    }
    public void SettingCharacterBtn()
    {
        // 현재 방의 설정을 담은 SettingPopup을 띄운다.
        UIManager._instance.OpenPopup(ePopup.CharacterPickWnd);
        _slots[_index].StateChange(ePlayerReadyState.SetUp);
    }
    public void StartGameBtn()
    {
        // 모든 룸의 참가자에게 Scene을 옮기라고 보낸다
        SettingManager._instance._myIndex = _index;
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (!_slots[i].CheckReady()) return;
        }
        GoDreamCatch();
    }
    public void ReadyBtn()
    {
        if (_slots[_index].CheckReady())
        {
            _readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "준비 완료";
            _slots[_index].StateChange(ePlayerReadyState.Wait);
            _exitBtn.interactable = true;
            _settingCharBtn.interactable = true;
        }
        else
        {
            _readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "준비 해제";
            _slots[_index].StateChange(ePlayerReadyState.Ready);
            _exitBtn.interactable = false;
            _settingCharBtn.interactable = false;
        }
    }
    public void ExitRoomBtn()
    {
        foreach (CharacterInfoSlot slot in _slots)
        {
            slot.gameObject.SetActive(false);
        }
        PhotonNetwork.LeaveRoom();
    }
    #endregion [ GameBtn Event ]

    #region [ 함수 ]
    public void MyStateChange(ePlayerReadyState state)
    {
        _slots[_index].StateChange(state);
    }
    public void CallRoomUpdate(string str = "")
    {
        _pv.RPC("RoomUpdate", RpcTarget.All);
        _pv.RPC("ChatRPC", RpcTarget.All, str);
    }
    public void ChatClear()
    {
        for (int i = 0; i < _content.transform.childCount; i++)
        {
            Destroy(_content.transform.GetChild(i).gameObject);
        }
    }
    #endregion [ 함수 ]

    #region [ RPC Function Section ]
    public void Send()
    {
        _pv.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + _chatInput.text);
        _chatInput.text = "";
    }
    public void Send(string str)
    {
        _pv.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.IsMasterClient + " / " + PhotonNetwork.NickName + str);
    }
    
    public void GoDreamCatch()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (!_slots[i].CheckReady()) return;
        }
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        _pv.RPC("LoadGameScene", RpcTarget.All, eGameScene.DreamCatch);
    }
    #endregion [ RPC Functino Section ]


    #region [ RPC ]
    [PunRPC]
    public void RoomUpdate()
    {
        // 제목칸 업데이트
        _title.text = PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.Title.ToString()].ToString();
        _roomNum.text = PhotonNetwork.CurrentRoom.Name;
        _mode.text = PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.Mode.ToString()].ToString();
       
        bool ready;
        bool setup;
        // 현재 내 index 업데이트
        if (_index > -1)
        {
            ready = _slots[_index].CheckReady();
            setup = _slots[_index].CheckSetUp();
        }
        else
        {
            ready = false;
            setup = false;
        }

        if (SettingManager._instance._myIndex != -1)
            _index = SettingManager._instance._myIndex;
        if (_index == -1)
            _index = PhotonNetwork.CurrentRoom.PlayerCount-1;
        else if (_index >= PhotonNetwork.CurrentRoom.PlayerCount)
            _index--;

        // Slots 업데이트
        _slots[_index].ActiveSlot(PhotonNetwork.LocalPlayer.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.LocalPlayer.CustomProperties[ePlayerProperty.Character.ToString()].ToString());

        for (int i = PhotonNetwork.CurrentRoom.PlayerCount; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            _slots[i].OffSlot();
        }

        // 상태에 따른 Stat Text 업데이트
        if (setup)
        {
            _slots[_index].StateChange(ePlayerReadyState.SetUp);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            _slots[_index].StateChange(ePlayerReadyState.Host);
        }
        else if (ready)
        {
            _slots[_index].StateChange(ePlayerReadyState.Ready);
        }
        else
        {
            _slots[_index].StateChange(ePlayerReadyState.Wait);
        }

        //상태에 따른 버튼 업데이트
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            _startBtn.interactable = true;
        }
        else
        {
            _startBtn.interactable = false;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            _readyBtn.gameObject.SetActive(false);
            _startBtn.gameObject.SetActive(true);
            _settingRoomBtn.gameObject.SetActive(true);
            _settingRoomBtn.interactable = true;
            _exitBtn.interactable = true;
        }
        else
        {
            _readyBtn.gameObject.SetActive(true);
            _startBtn.gameObject.SetActive(false);
            _settingRoomBtn.gameObject.SetActive(false);
            _settingRoomBtn.interactable = true;
            _exitBtn.interactable = true;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            if (!_slots[_index].CheckReady())
            {
                _readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "준비 완료";
                _slots[_index].StateChange(ePlayerReadyState.Wait);
                _exitBtn.interactable = true;
                _settingCharBtn.interactable = true;
            }
            else
            {
                _readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "준비 해제";
                _slots[_index].StateChange(ePlayerReadyState.Ready);
                _exitBtn.interactable = false;
                _settingCharBtn.interactable = false;
            }
        }

    }
    [PunRPC] // Chatting을 모든 PC에 뿌려준다.
    void ChatRPC(string text)
    {
        if (text == "") return;
        GameObject go = Instantiate(_prefabChatLine, _content.transform);
        go.GetComponent<TextMeshProUGUI>().text = text;
    }
    [PunRPC]
    void LoadGameScene(eGameScene scene)
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        PhotonNetwork.LoadLevel((int)scene);
    }
    #endregion[ RPC ]

    //void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();
    //    style.normal.textColor = Color.black;
    //    style.fontSize = 50;
    //    GUI.Label(new Rect(Screen.width / 2 - 500, 10, 500, 50), _index.ToString(), style);
    //}
}
