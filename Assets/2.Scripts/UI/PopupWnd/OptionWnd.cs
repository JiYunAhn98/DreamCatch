using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;

public class OptionWnd : BasePopupWnd
{
    [SerializeField] TMP_Dropdown _resolutionList;
    [SerializeField] Toggle _fullScreenCheckBox;
    [SerializeField] Slider _totalSlider;
    [SerializeField] Slider _bgmSlider;
    [SerializeField] Slider _effectSlider;
    [SerializeField] GameObject _logoutBtn;
    [SerializeField] GameObject _exitBtn;

    // 정보 변수
    List<Resolution> _resolutions;

    /// <summary>
    /// Wnd가 켜졌을 때 활성화된다. 초기 설정을 관리한다.
    /// </summary>
    /// <param name="resolutions"></param>
    public override void OpenWnd()
    {
        List<Resolution> resolutions = SettingManager._instance._resolutionList;

        if (_resolutions == null)
        {
            _resolutionList.ClearOptions();
            for (int i = 0; i < resolutions.Count; i++)
            {
                _resolutionList.options.Add(new TMP_Dropdown.OptionData(resolutions[i].ToString()));
                if (resolutions[i].width == SettingManager._instance._gameOption._resolution.x && resolutions[i].height == SettingManager._instance._gameOption._resolution.y)
                    SettingManager._instance._selectResolutionNumber = i;
            }
            _resolutions = resolutions;
        }

        _bgmSlider.value = SettingManager._instance._gameOption._bgmVol;
        _effectSlider.value = SettingManager._instance._gameOption._effectVol;
        _totalSlider.value = SettingManager._instance._gameOption._totalVol;
        _resolutionList.value = SettingManager._instance._selectResolutionNumber;
        _fullScreenCheckBox.isOn = SettingManager._instance._gameOption._fullScreen;

        if (PhotonNetwork.InRoom)
        {
            _logoutBtn.SetActive(false);
            _exitBtn.SetActive(false);
        }
        else if (PhotonNetwork.NetworkClientState == ClientState.PeerCreated || PhotonNetwork.NetworkClientState == ClientState.Disconnected)
        {
            _logoutBtn.SetActive(false);
            _exitBtn.SetActive(true);
        }
        else
        {
            _logoutBtn.SetActive(true);
            _exitBtn.SetActive(true);
        }
    }
    /// <summary>
    /// Yes버튼을 누를 때 활성화된다. 모든 설정을 저장한다.
    /// </summary>
    public override void ClickOK()
    {
        SettingManager._instance._gameOption.SettingAllOption(new Vector2Int(_resolutions[_resolutionList.value].width, _resolutions[_resolutionList.value].height), (int)_totalSlider.value, (int)_bgmSlider.value, (int)_effectSlider.value, _fullScreenCheckBox.isOn);
        SettingManager._instance._selectResolutionNumber = _resolutionList.value;

        Screen.SetResolution(_resolutions[_resolutionList.value].width, _resolutions[_resolutionList.value].height, _fullScreenCheckBox.isOn);

        UIManager._instance.ClosePopup(DefineHelper.ePopup.OptionWnd);
    }
    /// <summary>
    /// No버튼을 누를 때 활성화된다. 음량만 바뀐 것을 토대로 저장한다.
    /// </summary>
    public void ClickNo()
    {
        UIManager._instance.ClosePopup(DefineHelper.ePopup.OptionWnd);
    }
    /// <summary>
    /// TotalSlider의 Value가 변환되면 작동한다.
    /// </summary>
    public void MoveTotalSlider()
    {
        SettingManager._instance._gameOption._totalVol = (int)_totalSlider.value;
    }
    /// <summary>
    /// BGMSlider의 Value가 변환되면 작동한다.
    /// </summary>
    public void MoveBGMSlider()
    {
        SettingManager._instance._gameOption._bgmVol = (int)_bgmSlider.value;
    }
    /// <summary>
    /// EffectSlider의 Value가 변환되면 작동한다.
    /// </summary>
    public void MoveEffectSlider()
    {
        SettingManager._instance._gameOption._effectVol = (int)_effectSlider.value;
    }
    public void ClickLogOutBtn()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PhotonNetwork.Disconnect();
    }
    public void ClickExitBtn()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PhotonNetwork.Disconnect();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
