using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class MiniInform : MonoBehaviour
{
    [SerializeField] Slider _hpbar;
    [SerializeField] TextMeshProUGUI _nickName;
    PhotonView _pv;

    public void InitStatus(float hp, string name, Color col)
    {
        _pv = GetComponent<PhotonView>();
        _hpbar.maxValue = hp;
        _hpbar.value = hp;
        _nickName.text = name;
        _nickName.color = col;
        _pv.RPC("InitStatusRPC", RpcTarget.Others, hp, name);
    }

    [PunRPC]
    public void InitStatusRPC(float hp, string name)
    {
        _hpbar.maxValue = hp;
        _hpbar.value = hp;
        _nickName.text = name;
        _nickName.color = Color.red;
    }

    public void HPValue(float hp)
    {
        _hpbar.value = hp;
    }
}
