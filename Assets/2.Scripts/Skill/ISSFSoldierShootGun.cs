using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using Photon.Pun;

public class ISSFSoldierShootGun : CSkill
{
    Projectile _bullet;
    public override void GetInit(Transform player)
    {
        // 공격 볌위 트리거 가져오기
        int pickChar = (int)eCharacter.SFSoldier + 1;
        int skillKey = (int)eSkillIndex.Q;

        // 공격 볌위 트리거 가져오기
        float cool = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.CoolTime.ToString());
        float att = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.Damage.ToString());
        float coeff = TableManager._instance.Takefloat((TableManager.eTableJsonNames)pickChar, skillKey, TableManager.eSkillIndex.ATT.ToString());
        _skilStatus = new stSkill(att, coeff, (int)eAnimState.Q, cool, (int)eCrowdControl.Stuck);

        _pv = player.gameObject.GetComponent<PhotonView>();
        _bullet = Instantiate(Resources.Load("Projectile/Bullet") as GameObject, player.position, Quaternion.identity).GetComponent<Projectile>();
        _bullet.GetComponent<Projectile>().Initialize(_skilStatus, player);
    }
    public override void StartAttack()
    {
        //_pv.RPC("Fire", RpcTarget.AllViaServer, _point.position, _point.forward, 3);
        _bullet.ProjectileOn(1.5f);
    }
    public override void EndAttack()
    {
    }

    //[PunRPC]
    //public void Fire(Vector3 position, Quaternion rotation, float time)
    //{
    //    //float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

    //    //_bullet.GetComponent<Bullet>().InitializeBullet(GetComponent<PhotonView>().Owner, (rotation * Vector3.forward));//, Mathf.Abs(lag));
    //    _bullet.GetComponent<Bullet>().Initialize(transform.position, (rotation * Vector3.forward), time);//, Mathf.Abs(lag));
    //}
}
