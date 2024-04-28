using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using Photon.Pun;

public class ISSFSoldierThrowBomb : CSkill
{
    Bomb _bomb;
    Transform _point;
    public override void GetInit(Transform player)
    {        // 공격 볌위 트리거 가져오기
        int pickChar = (int)eCharacter.SFSoldier + 1;
        int skillKey = (int)eSkillIndex.R1;

        float cool = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.CoolTime.ToString());
        float att = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.Damage.ToString());
        float coeff = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.ATT.ToString());

        _skilStatus = new stSkill(att, coeff, (int)eAnimState.R, cool, (int)eCrowdControl.Down);

        _pv = player.gameObject.GetComponent<PhotonView>();
        _bomb = Instantiate(Resources.Load("Projectile/Bomb") as GameObject, player.position, Quaternion.identity).GetComponent<Bomb>();
        _bomb.Initialize(_skilStatus, player);
    }
    public override void StartAttack()
    {
        _bomb.ProjectileOn(3);
        //_pv.RPC("Fire", RpcTarget.AllViaServer, _point.position, _point.forward, 3);
    }
    public override void EndAttack()
    {
    }

    [PunRPC]
    public void Fire(Vector3 position, Quaternion rotation, float time)
    {
        //float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        //_bullet.GetComponent<Bullet>().InitializeBullet(GetComponent<PhotonView>().Owner, (rotation * Vector3.forward));//, Mathf.Abs(lag));
        //_bomb.GetComponent<Bomb>().Initialize(transform.position, (rotation * Vector3.forward), time);//, Mathf.Abs(lag));
    }
}
