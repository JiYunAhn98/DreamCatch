using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

// InRoom�̸� ExitŰ ������ ����, �ƴϸ� SetActive, ���� Master�� ���������� 0��° ������� Master����
public class UIManager : MonoSingleton<UIManager>
{
    GameObject[] _prefabPopups;
    Dictionary<ePopup, BasePopupWnd> _popups;

    public void PopupLoad() 
    {
        if (_popups == null)
        {
            _popups = new Dictionary<ePopup, BasePopupWnd>();
            _prefabPopups = new GameObject[(int)ePopup.Cnt];
            for (int i = 0; i < (int)ePopup.Cnt; i++)
            {
                _prefabPopups[i] = Resources.Load("UI/Popup/" + ((ePopup)i).ToString()) as GameObject;
            }
        }
    }
    public void OpenPopup(ePopup popupName)
    {
        if (_popups.ContainsKey(popupName))
        {
            _popups[popupName].gameObject.SetActive(true);
        }
        else
        {
            GameObject go = Instantiate(_prefabPopups[(int)popupName], transform);
            _popups.Add(popupName, go.GetComponent<BasePopupWnd>());
        }
        _popups[popupName].OpenWnd();
    }
    public void OpenAlarm(bool isGood, string message)
    {
        ePopup popupName = ePopup.AlarmWnd;
        if (_popups.ContainsKey(popupName))
        {
            _popups[popupName].gameObject.SetActive(true);
        }
        else
        {
            GameObject go = Instantiate(_prefabPopups[(int)popupName], transform);
            _popups.Add(popupName, go.GetComponent<BasePopupWnd>());
        }
        _popups[popupName].gameObject.GetComponent<AlarmWnd>().OpenWnd(isGood, message);
    }
    public void ClosePopup(ePopup popupName)
    {
        if (_popups.ContainsKey(popupName))
        {
            _popups[popupName].gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("�˾��� �������� �ʽ��ϴ�.");
            return;
        }
    }
    public void CloseAllPopup()
    {
        foreach (ePopup popupName in _popups.Keys)
        {
            if (popupName == ePopup.AlarmWnd) continue;
            _popups[popupName].gameObject.SetActive(false);
        }
    }
}
