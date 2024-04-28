using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSFSoldier : Character
{
    public override void InitSkill()
    {
        _pickChar = DefineHelper.eCharacter.SFSoldier;

        _leftClickSkill = new ISSFSoliderBaseAttack();
        _qSkill = new ISSFSoldierShootGun();
        _eSkill = new ISSFSoldierBoostDash();
        _rSkill = new ISSFSoldierThrowBomb();

        _leftClickSkill.GetInit(transform);
        _qSkill.GetInit(transform);
        _eSkill.GetInit(transform);
        _rSkill.GetInit(transform);
    }

}
