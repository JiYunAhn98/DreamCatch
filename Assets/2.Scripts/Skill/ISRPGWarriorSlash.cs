using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using Photon.Realtime;
using Photon.Pun;

public class ISRPGWarriorSlash : CSkill, IPunObservable
{
    public override void GetInit(Transform player)
    {
        // 공격 볌위 트리거 가져오기
        int pickChar = (int)eCharacter.RPGWarrior + 1;
        int skillKey = (int)eSkillIndex.R1;

        float cool = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.CoolTime.ToString());
        float att = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.Damage.ToString());
        float coeff = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.ATT.ToString());

        _skilStatus = new stSkill(att, coeff, (int)eAnimState.R, cool, (int)eCrowdControl.Down);
        _rNum = (int)eRPGWarriorRSkill.Slash;

        _pv = player.gameObject.GetComponent<PhotonView>();
        _boxTrigger = player.GetChild(0).GetComponentsInChildren<BoxCollider>()[3];
        _boxTrigger.center = new Vector3(0, 0, 1.5f);
        _boxTrigger.size = new Vector3(1.5f, 0.5f, 1.5f);

        TriggerSwitch(false);

    }
    public override void StartAttack()
    {
        // 트리거 on
        Debug.Log("Slash 판정 시작!");
        TriggerSwitch(true);
    }
    public override void EndAttack()
    {
        Debug.Log("Slash 판정 끝!");
        TriggerSwitch(false);
    }

    public void TriggerSwitch(bool isOn)
    {
        _pv.RPC("RPCTriggerSwitch", RpcTarget.AllViaServer, isOn, 3);
    }
    [PunRPC]
    public void RPCTriggerSwitch(bool isOn)
    {
        _boxTrigger.gameObject.SetActive(isOn);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_skilStatus);
        }
        else
        {
            _skilStatus = (stSkill)stream.ReceiveNext();
        }
    }
}
