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

    //[PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
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

        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < _texts.Length; i++) _texts[i - 1].text = _texts[i].text;
            _texts[_texts.Length - 1].text = msg;
        }
    }
}
