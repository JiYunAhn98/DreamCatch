using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DefineHelper;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomSettingWnd : BasePopupWnd
{
    [SerializeField] TMP_InputField _roomName;
    [SerializeField] Toggle _secretLock;
    [SerializeField] TMP_InputField _secretCode;
    [SerializeField] TMP_Dropdown _gameMode;
    [SerializeField] TMP_Dropdown _maxPlayer;

    //RoomOptions _roomOptions;
    //Hashtable table;
    public override void OpenWnd()
    {
        //_roomOptions = new RoomOptions();

        // 게임모드 수만큼 받아오기
        if (_gameMode.options.Count != (int)eGameMode.Cnt)
        {
            _gameMode.ClearOptions();
            for (int i = 0; i < (int)eGameMode.Cnt; i++)
            {
                _gameMode.options.Add(new TMP_Dropdown.OptionData(((eGameMode)i).ToString()));
            }
        }

        if (_maxPlayer.options.Count != 8)
        {
            _maxPlayer.ClearOptions();
            for (int i = 1; i <= 8; i++)
            {
                _maxPlayer.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
            }
        }

        // 기본 값 설정
        _roomName.text = "";
        _secretLock.isOn = false;
        _secretCode.gameObject.SetActive(false);
        _secretCode.text = "";
        _gameMode.value = 0;
        _maxPlayer.value = 7;

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("방에 들어와 있습니다.");

            _maxPlayer.value = PhotonNetwork.CurrentRoom.MaxPlayers - 1;

            if (PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.Title.ToString()] != null)
            {
                _roomName.text = PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.Title.ToString()].ToString();
            }
            else Debug.Log("RoomName 값이 존재하지 않습니다.");

            if (PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.Mode.ToString()] != null)
            {
                string tmpProperty = PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.Mode.ToString()].ToString();
                _gameMode.value = _gameMode.options.IndexOf(new TMP_Dropdown.OptionData(tmpProperty));
            }
            else Debug.Log("RoomMode 값이 존재하지 않습니다.");

            if (PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.SecretCode.ToString()] != null)
            {
                _secretCode.text = PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.SecretCode.ToString()].ToString();
                if (_secretCode.text == "")
                {
                    _secretLock.isOn = false;
                }
                else
                {
                    _secretLock.isOn = true;
                }
                _secretCode.gameObject.SetActive(_secretLock.isOn);
            }
            else Debug.Log("SecretCode 값이 존재하지 않습니다.");
        }
        else
        {
            Debug.Log("방에 들어와 있지 않습니다.");
        }
    }
    public override void ClickOK()
    {
        Debug.Log("setting");
        if (_roomName.text == "")
        {
            UIManager._instance.OpenAlarm(false, "이름을 입력해주세요!");
            return;
        }
        else if (_secretLock.isOn == true && _secretCode.text == "")
        {
            UIManager._instance.OpenAlarm(false, "방의 비밀번호를 입력해주세요!");
            return;
        }
        if (!_secretLock.isOn)
            _secretCode.text = "";

        //settingmanager에서도 되면 거기서 사용하자
        SettingManager._instance.RoomOptionSetting(_maxPlayer.value + 1, _roomName.text, _gameMode.options[_gameMode.value].text, _secretCode.text);
        
        if (PhotonNetwork.InRoom)
        {
             PhotonNetwork.CurrentRoom.SetCustomProperties(SettingManager._instance._roomOptions.CustomRoomProperties);
        }
        else
        {
            PhotonNetwork.CreateRoom(PhotonNetwork.CountOfRooms.ToString() + DateTime.Now, SettingManager._instance._roomOptions);
        }
        UIManager._instance.ClosePopup(ePopup.RoomSettingWnd);
    }
    public void ClickNO()
    {
        UIManager._instance.ClosePopup(ePopup.RoomSettingWnd);
    }

    public void SecretModeTurn()
    {
        _secretCode.gameObject.SetActive(_secretLock.isOn);
    }
}
