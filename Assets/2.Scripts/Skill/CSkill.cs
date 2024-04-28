using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using Photon.Realtime;
using Photon.Pun;

public abstract class CSkill : MonoBehaviour
{
    public stSkill _skilStatus;
    
    public int _rNum;
    public double _nowCool;
    public BoxCollider _boxTrigger;
    public PhotonView _pv;

    /// <summary>
    /// 스킬을 얻었을 때 실행
    /// </summary>
    /// BoxCollider range
    public abstract void GetInit(Transform player);

    /// <summary>
    /// 스킬을 사용할 때 실행
    /// </summary>
    public abstract void StartAttack();
    public abstract void EndAttack();
}
