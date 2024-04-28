using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using DefineHelper;
public class CharacterInfoSlot : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI _nickname;         // 닉네임
    [SerializeField] TextMeshProUGUI _ready;            // Ready 여부
    [SerializeField] ObjectSpawn _characterSpawn;       // 픽한 캐릭터

    PhotonView _pv;

    #region [ MonoBehaviour Callback ]
    void Awake()
    {
        _pv = GetComponent<PhotonView>();
        gameObject.SetActive(false);
    }
    #endregion [ MonoBehaviour Callback ]


    #region [ 외부 함수 ]
    public void ActiveSlot(string name, bool isMaster, string pick)
    {
        //gameObject.SetActive(true);
        //_nickname.text = name;
        _pv.RPC("ActiveSlotRPC", RpcTarget.All, name, isMaster, pick);
        //if (isMaster)
        //{
        //    _pv.RPC("PlayerStateRPC", RpcTarget.All, ePlayerReadyState.Host);
        //}
        //else
        //{
        //    _pv.RPC("PlayerStateRPC", RpcTarget.All, ePlayerReadyState.Wait);
        //}

        //_characterSpawn.InstanceCharacter((eCharacter)System.Enum.Parse(typeof(eCharacter), pick));
    }
    public void OffSlot()
    {
        _characterSpawn.HideCharacter();
        gameObject.SetActive(false);
    }
    public string NowChar()
    {
        return _characterSpawn._nowPickChar.ToString();
    }

    public void StateChange(ePlayerReadyState state)
    {
        if (PhotonNetwork.IsMasterClient && (state ==ePlayerReadyState.Wait || state == ePlayerReadyState.Ready))
        {
            _pv.RPC("PlayerStateRPC", RpcTarget.All, ePlayerReadyState.Host);
        }
        else
            _pv.RPC("PlayerStateRPC", RpcTarget.All, state);
    }
    public bool CheckReady()
    {
        return (_ready.text == ePlayerReadyState.Ready.ToString() || _ready.text == ePlayerReadyState.Host.ToString());
    }
    public bool CheckSetUp()
    {
        return (_ready.text == ePlayerReadyState.SetUp.ToString());
    }
    #endregion [ 외부 함수 ]

    #region [ RPC ]
    [PunRPC]
    void PlayerStateRPC(ePlayerReadyState state)
    {
        _ready.text = state.ToString();
    }
    [PunRPC]
    void ActiveSlotRPC(string name, bool isMaster, string pick)
    {
        gameObject.SetActive(true);
        _nickname.text = name;

        if (isMaster)
        {
            _ready.text = ePlayerReadyState.Host.ToString();
        }
        else
        {
            _ready.text = ePlayerReadyState.Wait.ToString();
        }

        _characterSpawn.InstanceCharacter((eCharacter)System.Enum.Parse(typeof(eCharacter), pick));
    }
    #endregion [ RPC ]
}
