using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class BGPanel : BasePanel
{
    [SerializeField] TextMeshProUGUI _dpTxt;
    [SerializeField] TextMeshProUGUI _cashTxt;
    [SerializeField] GameObject _btnOption;
    [SerializeField] GameObject _cashInfo;
    [SerializeField] GameObject _dpInfo;

    public override void MyTurnInit()
    {
        gameObject.SetActive(true);

        // PlayFab에서 재화 받아오기
        _dpTxt.text = "0";
        _cashTxt.text = "0";


        _btnOption.SetActive(true);
        if (PhotonNetwork.NetworkClientState != ClientState.PeerCreated && PhotonNetwork.NetworkClientState != ClientState.Disconnected)
        {
            _cashInfo.SetActive(true);
            _dpInfo.SetActive(true);
        }
        else
        {
            _cashInfo.SetActive(false);
            _dpInfo.SetActive(false);
        }
    }
}
