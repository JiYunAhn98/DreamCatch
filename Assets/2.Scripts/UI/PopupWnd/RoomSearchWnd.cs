using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using DefineHelper;
using TMPro;
public class RoomSearchWnd : BasePopupWnd
{
    [SerializeField] TMP_InputField _roomName;
    [SerializeField] TMP_Dropdown _gameMode;

    RoomListPanel _roomListPanel;
    public override void OpenWnd()
    {
        _roomName.text = "";

        _gameMode.ClearOptions();
        for (int i = 0; i < (int)eGameMode.Cnt; i++)
        {
            _gameMode.options.Add(new TMP_Dropdown.OptionData( ((eGameMode)i).ToString() ));
        }
        _gameMode.value = 0;

        _roomListPanel = GameObject.Find("RoomListPanel").GetComponent<RoomListPanel>();
    }

    public override void ClickOK()
    {
        _roomListPanel.SearchList(_roomName.text, _gameMode.options[_gameMode.value].ToString());
    }

    public void ClickNo()
    {
        UIManager._instance.ClosePopup(ePopup.RoomSearchWnd);
    }
}
