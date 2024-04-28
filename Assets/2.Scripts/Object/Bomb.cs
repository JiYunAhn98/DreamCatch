using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class Bomb : Projectile
{
    GameObject _boomSection;

    IEnumerator Move(float time)
    {
        yield return new WaitForSeconds(time);
        _boomSection.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _boomSection.SetActive(false);
        gameObject.SetActive(false);
    }

    public override void Initialize(stSkill skill, Transform player)
    {
        gameObject.SetActive(false);
        _player = player.GetComponent<Player>();
        _mySkill = skill;
        _boomSection = transform.GetChild(0).gameObject;
        _boomSection.SetActive(false);
    }
    public override void ProjectileOn(float time)
    {
        Transform point = _player.transform.GetChild(0).GetComponentsInChildren<Transform>()[2];
        gameObject.SetActive(true);

        transform.position = point.position;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(point.forward * 2, ForceMode.Impulse);

        StartCoroutine(Move(time));
    }
}
