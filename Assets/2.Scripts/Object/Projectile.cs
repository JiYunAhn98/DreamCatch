using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public abstract class Projectile : MonoBehaviour
{
    public stSkill _mySkill;
    public Player _player;
    public abstract void Initialize(stSkill skill, Transform pos);
    public abstract void ProjectileOn(float time);
}
