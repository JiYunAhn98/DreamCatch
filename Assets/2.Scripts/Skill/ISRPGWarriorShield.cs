using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using Photon.Realtime;
using Photon.Pun;

public class ISRPGWarriorShield : CSkill
{
    public override void GetInit(Transform player)
    {
        // 공격 볌위 트리거 가져오기
        float cool = TableManager._instance.Takefloat((TableManager.eTableJsonNames)(eCharacter.RPGWarrior + 1), (int)eSkillIndex.R2, TableManager.eSkillIndex.CoolTime.ToString());
        _skilStatus = new stSkill(0, 0, (int)eAnimState.R, cool);
        _rNum = (int)eRPGWarriorRSkill.Shield;

        _pv = player.gameObject.GetComponent<PhotonView>();
        Debug.Log("설정완료");
    }
    public override void StartAttack()
    {
        // 트리거 on
        Debug.Log("Slash 판정 시작!");
    }
    public override void EndAttack()
    {
        Debug.Log("Slash 판정 끝!");
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
