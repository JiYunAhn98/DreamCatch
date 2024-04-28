using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

/// <summary>
/// PeerCreated ����
/// </summary>
public class LoginPanel : BasePanel
{
    [SerializeField] TMP_InputField _idInput;
    [SerializeField] TMP_InputField _pwInput;

    string _myId;

    public override void MyTurnInit()
    {
        gameObject.SetActive(true);
        _idInput.text = "";
        _pwInput.text = "";
    }
    public void LoginBtn()
    {
        if (_idInput.text == "")
        {
            UIManager._instance.OpenAlarm(false, "ID�� �Է��ϼ���!");
        }
        else if (_pwInput.text == "")
        {
            UIManager._instance.OpenAlarm(false, "Password�� �Է��ϼ���!");
        }
        else
        {
            var request = new LoginWithPlayFabRequest { Username = _idInput.text, Password = _pwInput.text };
            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
            // Ŀ���� ������Ƽ�� �̿��ؼ� 
        }
    }
    private void OnLoginSuccess(LoginResult result)
    {
        var request = new GetAccountInfoRequest { Username = _idInput.text };
        PlayFabClientAPI.GetAccountInfo(request, GetAccountSuccess, OnLoginFailure);
        PhotonNetwork.ConnectUsingSettings();
        UIManager._instance.OpenAlarm(true, "�α��� ����!");
    }
    private void GetAccountSuccess(GetAccountInfoResult result)
    {
        PhotonNetwork.NickName = result.AccountInfo.TitleInfo.DisplayName;
    }
    private void OnLoginFailure(PlayFabError error)
    {
        UIManager._instance.OpenAlarm(false, "�α��� ����\n" + error);
    }
    public void RegisterBtn()
    {
        UIManager._instance.OpenPopup(DefineHelper.ePopup.AccountRegisterWnd);
    }
}
