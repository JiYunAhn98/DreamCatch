using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

/// <summary>
/// Connected To Master ป๓ลย
/// </summary>
public class LobbyPanel : BasePanel
{
    [SerializeField] TextMeshProUGUI _nickNameTxt;

    public override void MyTurnInit()
    {
        gameObject.SetActive(true);
        SettingManager._instance._myIndex = -1;
        _nickNameTxt.text = PhotonNetwork.NickName;
    }
    public void ChannelMatchingBtn()
    {
        PhotonNetwork.JoinLobby();
    }
}
