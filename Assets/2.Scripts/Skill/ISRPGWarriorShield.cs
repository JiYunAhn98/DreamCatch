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
        // ���� ���� Ʈ���� ��������
        float cool = TableManager._instance.Takefloat((TableManager.eTableJsonNames)(eCharacter.RPGWarrior + 1), (int)eSkillIndex.R2, TableManager.eSkillIndex.CoolTime.ToString());
        _skilStatus = new stSkill(0, 0, (int)eAnimState.R, cool);
        _rNum = (int)eRPGWarriorRSkill.Shield;

        _pv = player.gameObject.GetComponent<PhotonView>();
        Debug.Log("�����Ϸ�");
    }
    public override void StartAttack()
    {
        // Ʈ���� on
        Debug.Log("Slash ���� ����!");
    }
    public override void EndAttack()
    {
        Debug.Log("Slash ���� ��!");
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
