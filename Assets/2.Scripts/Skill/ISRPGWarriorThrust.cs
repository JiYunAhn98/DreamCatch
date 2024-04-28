using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using Photon.Realtime;
using Photon.Pun;

public class ISRPGWarriorThrust : CSkill
{
    Transform _player;
    public override void GetInit(Transform player)
    {
        // 공격 볌위 트리거 가져오기
        _player = player;
        int pickChar = (int)eCharacter.RPGWarrior + 1;
        int skillKey = (int)eSkillIndex.E;

        float cool = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.CoolTime.ToString());
        float att = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.Damage.ToString());
        float coeff = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.ATT.ToString());

         _skilStatus = new stSkill(att, coeff, (int)eAnimState.E, cool, (int)eCrowdControl.Down);
        _pv = player.gameObject.GetComponent<PhotonView>();
        _boxTrigger = player.GetChild(0).GetComponentsInChildren<BoxCollider>()[2];

        TriggerSwitch(false);
    }
    public override void StartAttack()
    {
        // 트리거 on
        _player.GetComponent<Rigidbody>().AddForce(_player.forward * 60, ForceMode.Impulse);
        TriggerSwitch(true);
    }
    public override void EndAttack()
    {
        TriggerSwitch(false);
    }

    public void TriggerSwitch(bool isOn)
    {
        _pv.RPC("RPCTriggerSwitch", RpcTarget.AllViaServer, isOn, 2);
    }
    [PunRPC]
    public void RPCTriggerSwitch(bool isOn)
    {
        _boxTrigger.gameObject.SetActive(isOn);
    }
}
