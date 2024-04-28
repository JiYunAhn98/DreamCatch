using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class ISSFSoldierBoostDash : CSkill
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

        _skilStatus = new stSkill(att, coeff, (int)eAnimState.E, cool);
    }
    public override void StartAttack()
    {
        _player.GetComponent<Rigidbody>().AddForce(_player.forward * 55, ForceMode.Impulse);
    }
    public override void EndAttack()
    { 
    }
}
