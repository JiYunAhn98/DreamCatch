using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class SettingManager : MonoSingleton<SettingManager>
{
    // ��������
    List<Resolution> _resolutions;      // ������ �ػ� �ɼ� ��ü
    public stOptionData _gameOption;    // ���� �÷��̿� �ʿ��� ������ ���� ����ü
  
    public int _selectResolutionNumber { get; set; }
    public List<Resolution> _resolutionList { get { return _resolutions; } }
    public RoomOptions _roomOptions { get; set; }          // ����  �� �ɼ�
    public int _myIndex { get; set; }                       // ���� �濡�� ���� index

    public void SettingStart()
    {
        // ���� ������ Json���� �Ľ��� �����͸� �ҷ���
        TableManager._instance.CreateFiles();

        // �������� �޾ƿ´�
        stOptionData baseOption = new stOptionData(new Vector2Int(1920, 1080), 255, 255, 255, false);

        int index = -1;
        _myIndex = -1;
        _resolutions = new List<Resolution>();
        _roomOptions = new RoomOptions();

        foreach (Resolution resol in Screen.resolutions)
        {
            if (_resolutions.Count == 0 || resol.width != _resolutions[index].width || resol.height != _resolutions[index].height)
            {
                _resolutions.Add(resol);
                index++;
            }
            else if (resol.refreshRate > _resolutions[index].refreshRate)
                _resolutions[index] = resol;
        }

        _gameOption = new stOptionData(baseOption._resolution, baseOption._totalVol, baseOption._bgmVol, baseOption._effectVol, baseOption._fullScreen);
        Screen.SetResolution(_gameOption._resolution.x, _gameOption._resolution.y, _gameOption._fullScreen);
    }
    public void RoomOptionSetting(int maxPlayer, string roomNickName, string gameMode, string secretCode = "")
    {
        Hashtable table = new Hashtable();


        if (PhotonNetwork.InRoom)
        {
            Debug.Log(_roomOptions.CustomRoomProperties[eRoomProperty.Title.ToString()]);
            Debug.Log(_roomOptions.CustomRoomProperties[eRoomProperty.SecretCode.ToString()]);
            Debug.Log(_roomOptions.CustomRoomProperties[eRoomProperty.Mode.ToString()]);
            if (maxPlayer < PhotonNetwork.CurrentRoom.PlayerCount)  // ���� �ο����� ���� �ο��� ������ ���� �߻�
                PhotonNetwork.CurrentRoom.MaxPlayers = maxPlayer;
            //else return;

            table = PhotonNetwork.CurrentRoom.CustomProperties;
        }
        else
        {
            _roomOptions.MaxPlayers = maxPlayer;
        }

        _roomOptions.IsVisible = true;
        _roomOptions.IsOpen = true;

        table[eRoomProperty.Title.ToString()] = roomNickName;
        table[eRoomProperty.SecretCode.ToString()] = secretCode;
        table[eRoomProperty.Mode.ToString()] = gameMode;

        //table.Add(eRoomProperty.Title.ToString(), roomNickName);
        //table.Add(eRoomProperty.SecretCode.ToString(), secretCode);
        //table.Add(eRoomProperty.Mode.ToString(), gameMode);

        _roomOptions.CustomRoomProperties = table;

        string[] lobbyList = new string[(int)eRoomProperty.Cnt];

        for (int i = 0; i < (int)eRoomProperty.Cnt; i++)
            lobbyList[i] = ((eRoomProperty)(i)).ToString();

        _roomOptions.CustomRoomPropertiesForLobby = lobbyList;

        Debug.Log(_roomOptions.CustomRoomProperties[eRoomProperty.Title.ToString()]);
        Debug.Log(_roomOptions.CustomRoomProperties[eRoomProperty.SecretCode.ToString()]);
        Debug.Log(_roomOptions.CustomRoomProperties[eRoomProperty.Mode.ToString()]);
    }
}
