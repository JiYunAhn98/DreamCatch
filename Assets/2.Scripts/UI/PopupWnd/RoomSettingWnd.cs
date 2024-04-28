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

        // ���Ӹ�� ����ŭ �޾ƿ���
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

        // �⺻ �� ����
        _roomName.text = "";
        _secretLock.isOn = false;
        _secretCode.gameObject.SetActive(false);
        _secretCode.text = "";
        _gameMode.value = 0;
        _maxPlayer.value = 7;

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("�濡 ���� �ֽ��ϴ�.");

            _maxPlayer.value = PhotonNetwork.CurrentRoom.MaxPlayers - 1;

            if (PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.Title.ToString()] != null)
            {
                _roomName.text = PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.Title.ToString()].ToString();
            }
            else Debug.Log("RoomName ���� �������� �ʽ��ϴ�.");

            if (PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.Mode.ToString()] != null)
            {
                string tmpProperty = PhotonNetwork.CurrentRoom.CustomProperties[eRoomProperty.Mode.ToString()].ToString();
                _gameMode.value = _gameMode.options.IndexOf(new TMP_Dropdown.OptionData(tmpProperty));
            }
            else Debug.Log("RoomMode ���� �������� �ʽ��ϴ�.");

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
            else Debug.Log("SecretCode ���� �������� �ʽ��ϴ�.");
        }
        else
        {
            Debug.Log("�濡 ���� ���� �ʽ��ϴ�.");
        }
    }
    public override void ClickOK()
    {
        Debug.Log("setting");
        if (_roomName.text == "")
        {
            UIManager._instance.OpenAlarm(false, "�̸��� �Է����ּ���!");
            return;
        }
        else if (_secretLock.isOn == true && _secretCode.text == "")
        {
            UIManager._instance.OpenAlarm(false, "���� ��й�ȣ�� �Է����ּ���!");
            return;
        }
        if (!_secretLock.isOn)
            _secretCode.text = "";

        //settingmanager������ �Ǹ� �ű⼭ �������
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
