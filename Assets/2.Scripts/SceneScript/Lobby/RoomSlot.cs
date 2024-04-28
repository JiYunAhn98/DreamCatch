using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using DefineHelper;

public class RoomSlot : MonoBehaviour
{
    [SerializeField] Button _myself;
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] TextMeshProUGUI _mode;
    [SerializeField] TextMeshProUGUI _players;
    [SerializeField] GameObject _lock;

    public void OnSlot(RoomInfo option)
    {
        //_roomName = option.Name;
        _myself.interactable = true;
        transform.GetChild(0).gameObject.SetActive(true);
        _players.text = option.PlayerCount + "/" + option.MaxPlayers;

        if (option.CustomProperties.ContainsKey(eRoomProperty.Title.ToString()))
        {
            _title.text = option.CustomProperties[eRoomProperty.Title.ToString()].ToString();
        }
        else Debug.Log("RoomSlot : ���� �濡 RoomNickName ���� �������� �ʽ��ϴ�.");

        if (option.CustomProperties.ContainsKey(eRoomProperty.Mode.ToString()))
        {
            _mode.text = option.CustomProperties[eRoomProperty.Mode.ToString()].ToString();
        }
        else Debug.Log("RoomSlot : ���� �濡 RoomMode ���� �������� �ʽ��ϴ�.");

        if (option.CustomProperties.ContainsKey(eRoomProperty.SecretCode.ToString()))
        {
            if (option.CustomProperties[eRoomProperty.SecretCode.ToString()].ToString() == "")
            {
                _lock.SetActive(false);
            }
            else
            {
                _lock.SetActive(true);
            }
        }
        else Debug.Log("RoomSlot : ���� �濡 SecretCode ���� �������� �ʽ��ϴ�.");
    }
    public void OffSlot()
    {
        _myself.interactable = false;
        transform.GetChild(0).gameObject.SetActive(false);
        _title.text = "";
        _players.text = "";
    }
}
