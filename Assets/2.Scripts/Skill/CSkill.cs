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
    /// ��ų�� ����� �� ����
    /// </summary>
    /// BoxCollider range
    public abstract void GetInit(Transform player);

    /// <summary>
    /// ��ų�� ����� �� ����
    /// </summary>
    public abstract void StartAttack();
    public abstract void EndAttack();
}
