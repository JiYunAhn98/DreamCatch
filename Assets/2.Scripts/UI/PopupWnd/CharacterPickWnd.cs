using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using DefineHelper;
public class CharacterPickWnd : BasePopupWnd
{
    [SerializeField] TMP_Dropdown _charPick;
    [SerializeField] TMP_Dropdown _itemPick;

    ObjectSpawn _spawner;
    InRoomPanel _roomPanel;
    eCharacter _nowPickChar;

    public override void OpenWnd()
    {
        _spawner = GameObject.Find("CharacterSpawnPoint").GetComponent<ObjectSpawn>();
        if (PhotonNetwork.InRoom)
            _roomPanel = GameObject.Find("InRoomPanel").GetComponent<InRoomPanel>();

        List<TMP_Dropdown.OptionData> tmpOptions = new List<TMP_Dropdown.OptionData>();

        _charPick.options.Clear();
        for (int i = 0; i < (int)eCharacter.Cnt; i++)
        {
            tmpOptions.Add(new TMP_Dropdown.OptionData(((eCharacter)i).ToString()));
            if (tmpOptions[i].ToString() == PhotonNetwork.LocalPlayer.CustomProperties[ePlayerProperty.Character.ToString()].ToString())
                _charPick.value = i;
        }
        _charPick.AddOptions(tmpOptions);

        tmpOptions.Clear();
        
        //for (int i = 0; i < (int)eItem.Cnt; i++)
        //{
        //    options.Add(new TMP_Dropdown.OptionData(((eCharacter)i).ToString()));
        //}
        //_charPick.AddOptions(options);
    }

    public override void ClickOK()
    {
        PhotonNetwork.LocalPlayer.CustomProperties[ePlayerProperty.Character.ToString()] = _nowPickChar.ToString();

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
                _roomPanel.MyStateChange(ePlayerReadyState.Host);
            else
                _roomPanel.MyStateChange(ePlayerReadyState.Wait);
            _roomPanel.CallRoomUpdate();
        }
        UIManager._instance.ClosePopup(ePopup.CharacterPickWnd);
    }

    public void ClickNo()
    {
        ClickRewind();

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
                _roomPanel.MyStateChange(ePlayerReadyState.Host);
            else
                _roomPanel.MyStateChange(ePlayerReadyState.Wait);
        }
        UIManager._instance.ClosePopup(ePopup.CharacterPickWnd);
    }

    public void ClickRewind()
    {
        _nowPickChar = (eCharacter)System.Enum.Parse(typeof(eCharacter), PhotonNetwork.LocalPlayer.CustomProperties[ePlayerProperty.Character.ToString()].ToString());
        _charPick.value = (int)_nowPickChar;
        _spawner.InstanceCharacter(_nowPickChar);
    }

    public void CharacterChange()
    {
        _nowPickChar = (eCharacter)_charPick.value;
        Debug.Log(_nowPickChar);
        _spawner.InstanceCharacter(_nowPickChar);
    }
}
