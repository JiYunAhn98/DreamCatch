using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using Photon.Realtime;
using Photon.Pun;

public class ISRPGWarriorUpper : CSkill
{
    public override void GetInit(Transform player)
    {
        // ���� ���� Ʈ���� ��������
        int pickChar = (int)eCharacter.RPGWarrior + 1;
        int skillKey = (int)eSkillIndex.Q;

        // ���� ���� Ʈ���� ��������
        float cool = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.CoolTime.ToString());
        float att = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.Damage.ToString());
        float coeff = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.ATT.ToString());
        _skilStatus = new stSkill(att, coeff, (int)eAnimState.Q, cool, (int)eCrowdControl.Airborne);
        _pv = player.gameObject.GetComponent<PhotonView>();
        _boxTrigger = player.GetChild(0).GetComponentsInChildren<BoxCollider>()[1];

        TriggerSwitch(false);
    }
    public override void StartAttack()
    {
        // Ʈ���� on
        TriggerSwitch(true);
    }
    public override void EndAttack()
    {
        TriggerSwitch(false);
    }

    public void TriggerSwitch(bool isOn)
    {
        _pv.RPC("RPCTriggerSwitch", RpcTarget.Others, isOn, 1);
    }
    [PunRPC]
    public void RPCTriggerSwitch(bool isOn)
    {
        _boxTrigger.gameObject.SetActive(isOn);
    }
}