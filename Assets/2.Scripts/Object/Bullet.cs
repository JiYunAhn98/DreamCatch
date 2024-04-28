using Photon.Realtime;
using UnityEngine;
using System.Collections;
using DefineHelper;

public class Bullet : Projectile
{
    public void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Move(0.1f));
    }
    IEnumerator Move(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    public override void Initialize(stSkill skill, Transform player)
    {
        _mySkill = skill;
        _player = player.GetComponent<Player>();
        gameObject.SetActive(false);
    }
    public override void ProjectileOn(float time)
    {
        Transform point = _player.transform.GetChild(0).GetComponentsInChildren<Transform>()[1];

        gameObject.SetActive(true);
        transform.position = point.position;
        transform.forward = point.forward;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = point.forward * 10.0f;
        StartCoroutine(Move(time));
    }
}