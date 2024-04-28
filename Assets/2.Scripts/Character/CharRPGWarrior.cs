using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharRPGWarrior : Character
{
    public override void InitSkill()
    {
        _pickChar = DefineHelper.eCharacter.RPGWarrior;

        _leftClickSkill = new ISRPGWarriorBaseAttack();
        _qSkill = new ISRPGWarriorUpper();
        _eSkill = new ISRPGWarriorThrust();
        _rSkill = new ISRPGWarriorSlash();

        _leftClickSkill.GetInit(transform);
        _qSkill.GetInit(transform);
        _eSkill.GetInit(transform);
        _rSkill.GetInit(transform);
    }
}
