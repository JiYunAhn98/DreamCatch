using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

/// <summary>
/// PeerCreated 상태
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
            UIManager._instance.OpenAlarm(false, "ID를 입력하세요!");
        }
        else if (_pwInput.text == "")
        {
            UIManager._instance.OpenAlarm(false, "Password를 입력하세요!");
        }
        else
        {
            var request = new LoginWithPlayFabRequest { Username = _idInput.text, Password = _pwInput.text };
            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
            // 커스텀 프로퍼티를 이용해서 
        }
    }
    private void OnLoginSuccess(LoginResult result)
    {
        var request = new GetAccountInfoRequest { Username = _idInput.text };
        PlayFabClientAPI.GetAccountInfo(request, GetAccountSuccess, OnLoginFailure);
        PhotonNetwork.ConnectUsingSettings();
        UIManager._instance.OpenAlarm(true, "로그인 성공!");
    }
    private void GetAccountSuccess(GetAccountInfoResult result)
    {
        PhotonNetwork.NickName = result.AccountInfo.TitleInfo.DisplayName;
    }
    private void OnLoginFailure(PlayFabError error)
    {
        UIManager._instance.OpenAlarm(false, "로그인 실패\n" + error);
    }
    public void RegisterBtn()
    {
        UIManager._instance.OpenPopup(DefineHelper.ePopup.AccountRegisterWnd);
    }
}
