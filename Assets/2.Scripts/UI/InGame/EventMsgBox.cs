using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class EventMsgBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] _texts;
    [SerializeField] PhotonView _pv;

    public void EventOccur(string str)
    {
        //_pv.RPC("ChatRPC", RpcTarget.All , str);
        ChatRPC(str);
    }

    //[PunRPC] // RPC�� �÷��̾ �����ִ� �� ��� �ο����� �����Ѵ�
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < _texts.Length; i++)
        {
            if (_texts[i].text == "")
            {
                isInput = true;
                _texts[i].text = msg;
                break;
            }
        }

        if (!isInput) // ������ ��ĭ�� ���� �ø�
        {
            for (int i = 1; i < _texts.Length; i++) _texts[i - 1].text = _texts[i].text;
            _texts[_texts.Length - 1].text = msg;
        }
    }
}
