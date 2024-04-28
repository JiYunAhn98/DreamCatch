using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using Photon.Realtime;
using Photon.Pun;

public class ISRPGWarriorBaseAttack : CSkill
{
    public override void GetInit(Transform player)
    {
        // 공격 볌위 트리거 가져오기
        int pickChar = (int)eCharacter.RPGWarrior + 1;
        int skillKey = (int)eSkillIndex.LeftClick;

        float cool = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.CoolTime.ToString());
        float att = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.Damage.ToString());
        float coeff = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.ATT.ToString());

        _skilStatus = new stSkill(att, coeff, (int)eAnimState.MouseLeft, cool, (int)eCrowdControl.Stuck);
        _pv = player.gameObject.GetComponent<PhotonView>();
        _boxTrigger = player.GetChild(0).GetComponentsInChildren<BoxCollider>()[0];

        TriggerSwitch(false);
    }
    public override void StartAttack()
    {
        // 트리거 on
        TriggerSwitch(true);
    }
    public override void EndAttack()
    {
        TriggerSwitch(false);
    }

    public void TriggerSwitch(bool isOn)
    {
        _pv.RPC("RPCTriggerSwitch", RpcTarget.AllViaServer, isOn, 0);
    }
    [PunRPC]
    public void RPCTriggerSwitch(bool isOn, int num)
    {
        _boxTrigger.gameObject.SetActive(isOn);
    }
}
