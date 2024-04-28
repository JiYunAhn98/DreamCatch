using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public abstract class Character : MonoBehaviour
{
    [HideInInspector] public CSkill _qSkill;
    [HideInInspector] public CSkill _eSkill;
    [HideInInspector] public CSkill _leftClickSkill;
    [HideInInspector] public CSkill _rightClickSkill;
    [HideInInspector] public CSkill _rSkill;

    [HideInInspector] public eCharacter _pickChar;

    public abstract void InitSkill();
}
