using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AlarmWnd : BasePopupWnd
{
    [SerializeField] TextMeshProUGUI _message;
    [SerializeField] GameObject _badIcon;
    [SerializeField] GameObject _goodIcon;

    public override void OpenWnd()
    {
        _message.text = "Error";
    }
    public void OpenWnd (bool isGood, string message)
    {
        _message.text = message;
        _badIcon.SetActive(!isGood);
        _goodIcon.SetActive(isGood);
    }

    public override void ClickOK()
    {
        UIManager._instance.ClosePopup(DefineHelper.ePopup.AlarmWnd);
    }
}
