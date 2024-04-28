using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefineHelper;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

/// <summary>
/// Joined Lobby ����
/// </summary>
public class RoomListPanel : BasePanel
{
    [SerializeField] RoomSlot[] _cellBtn;
    [SerializeField] Button _prevBtn;
    [SerializeField] Button _nextBtn;
    [SerializeField] TextMeshProUGUI _nickname;

    [Header("PasswordWnd")]
    [SerializeField] GameObject _passwordWnd;
    [SerializeField] TMP_InputField _password;
    [SerializeField] TextMeshProUGUI _roomName;

    // ������ ���� ��� ������ �ִ´�. �������� �ʰ� ���� �ٽ� �����ϴ� ���
    List<RoomInfo> _myList;
    int currentPage;
    int _nowRoomIndex;
    int maxPage;
    int multiple;   //�� ù�������� ù���� ����Ŵ
    bool _listUpdateTrigger;

    public override void MyTurnInit()
    {
        gameObject.SetActive(true);
        _passwordWnd.SetActive(false);
        _myList = new List<RoomInfo>();
        _nickname.text = PhotonNetwork.NickName;
        _listUpdateTrigger = true;
        foreach (RoomSlot slot in _cellBtn)
        {
            slot.transform.GetChild(1).gameObject.SetActive(false);
        }
        currentPage = 1;
    }

    #region [ Btn Method ]
    public void QuickMatchBtn()
    {
        PhotonNetwork.JoinRandomRoom(SettingManager._instance._roomOptions.CustomRoomProperties, (byte)SettingManager._instance._roomOptions.MaxPlayers);
    }
    public void RefreshBtn()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.JoinedLobby)
            PhotonNetwork.JoinLobby();
        currentPage = 1;
        MyListRenewal();
    }
    public void ChannelOptionBtn()
    {
        UIManager._instance.OpenPopup(ePopup.RoomSearchWnd);
    }
    public void CreateRoomBtn()
    {
        UIManager._instance.OpenPopup(ePopup.RoomSettingWnd);
    }
    public void EnterRoomBtn()
    {
        if (_nowRoomIndex < 0)
            return;

        if (_myList[_nowRoomIndex].CustomProperties == null || _password.text == _myList[_nowRoomIndex].CustomProperties[eRoomProperty.SecretCode.ToString()].ToString())
            PhotonNetwork.JoinRoom(_myList[_nowRoomIndex].Name);
        else
        {
            _roomName.text = _myList[_nowRoomIndex].CustomProperties[eRoomProperty.Title.ToString()].ToString();
            _password.text = "";
            _passwordWnd.SetActive(true);
        }
    }
    public void PasswordWndClickNo()
    {
        _passwordWnd.SetActive(false);
    }
    public void ExitBtn()
    {
        PhotonNetwork.LeaveLobby();
    }
    #endregion [ Btn Method ]
    public void SyncRoomList(List<RoomInfo> roomInfo)
    {
        _myList = roomInfo;
        if (_listUpdateTrigger)
        {
            MyListRenewal();
            _listUpdateTrigger = false;
        }
    }
    public void SearchList(string roomName, string gameMode)
    {
        foreach (RoomInfo room in _myList)
        {
            if (room.CustomProperties[eRoomProperty.Mode].ToString() == roomName || room.CustomProperties[eRoomProperty.Mode].ToString() == gameMode)
            {
                _myList.Remove(room);
            }
        }
        MyListRenewal();
    }
    public void MyListClick(int num)
    {
        if (num == -2)
        {
            --currentPage;
            MyListRenewal();
        }
        else if (num == -1)
        {
            ++currentPage;
            MyListRenewal();
        }
        else
        {
            _nowRoomIndex = multiple + num;
            _cellBtn[num].transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    public void MyListRenewal()
    {
        OtherClick();
        // �ִ�������
        maxPage = (_myList.Count % _cellBtn.Length == 0) ? _myList.Count / _cellBtn.Length : _myList.Count / _cellBtn.Length + 1;

        // ����, ������ư
        _prevBtn.interactable = (currentPage <= 1) ? false : true;
        _nextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // �������� �´� ����Ʈ ����
        multiple = (currentPage - 1) * _cellBtn.Length;
        for (int i = 0; i < _cellBtn.Length; i++)
        {
            if (multiple + i < _myList.Count)
            {
                _cellBtn[i].OnSlot(_myList[multiple + i]);
            }
            else
            {
                _cellBtn[i].OffSlot();
            }
        }
    }
    public void OtherClick()
    {
        if (_nowRoomIndex > -1)
        {
            _cellBtn[_nowRoomIndex - multiple].transform.GetChild(1).gameObject.SetActive(false);
        }
        _nowRoomIndex = -1;
    }
}
