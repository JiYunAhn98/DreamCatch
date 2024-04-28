using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class AccountRegisterWnd : BasePopupWnd
{
    [SerializeField] TMP_InputField _id;
    [SerializeField] TMP_InputField _pw;
    [SerializeField] TMP_InputField _nickName;
    [SerializeField] TMP_InputField _email;

    public override void OpenWnd()
    {
        _id.text = "";
        _pw.text = "";
        _nickName.text = "";
        _email.text = "";
    }
    public override void ClickOK()
    {
        if (_id.text == "" || _pw.text == "" || _nickName.text == "" || _email.text == "")
            UIManager._instance.OpenAlarm(false, "빈 칸을 입력해주세요!");
        else
        {
            // 파일에 계정을 저장
            var registerRequest = new RegisterPlayFabUserRequest { Username = _id.text, Password = _pw.text, DisplayName = _nickName.text, Email = _email.text };
            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegistSuccess, OnRegistFailure);
        }
    }
    private void OnRegistSuccess(RegisterPlayFabUserResult result)
    {
        UIManager._instance.OpenAlarm(true, "등록에 성공했습니다!.");
        UIManager._instance.ClosePopup(DefineHelper.ePopup.AccountRegisterWnd);
    }
    private void OnRegistFailure(PlayFabError error)
    {
        UIManager._instance.OpenAlarm(false, "회원가입 실패\n" + error);
    }
    public void ClickCancle()
    {
        UIManager._instance.ClosePopup(DefineHelper.ePopup.AccountRegisterWnd);
    }
}
