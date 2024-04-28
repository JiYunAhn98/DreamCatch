using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using DefineHelper;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Text;


public class Player : MonoBehaviourPunCallbacks ,IPunObservable
{
    #region [ 변수 ]
    // 참조 변수
    [SerializeField] MiniInform _miniStatus;
    [SerializeField] Rigidbody _rigid;
    [SerializeField] Animator _animator;
    [SerializeField] PhotonView _pv; //포톤뷰에 연결된 컴포넌트의 핵심: 컴포넌트가 연결되어있다면 그 컴포넌트의 움직임은 모두 알아서 동기화된다.
    [SerializeField] BoxCollider[] _boxTriggers;
    [SerializeField] Material _dissolve;
    MyStatusBox _mainStatusBox;

    // 상태 변수
    bool isGround;
    Vector3 _dir;
    int _nowDamage;
    CSkill _nowSkill;

    // 캐릭터 스탯 변수
    float _maxHp;
    float _nowHp;
    float _att;
    float _speed;
    float _def;
    float _jumpPower = 5;
    bool _isSkill;
    bool _isJump;
    bool _isDeath;
    bool _isInvincible;
    bool _isStuck;
    bool _isDown;
    eCharacter _nowChar;

    Character _pick;

    #endregion [ 변수 ]

    void Update()
    {
        if (_pv.IsMine && AllhasTag("Life"))
        {
            double nowTime = PhotonNetwork.Time;

            if (nowTime >= _pick._qSkill._nowCool)
            {
                _pick._qSkill._nowCool = 0;
            }
            if (nowTime >= _pick._eSkill._nowCool)
            {
                _pick._eSkill._nowCool = 0;
            }
            if (nowTime >= _pick._leftClickSkill._nowCool)
            {
                _pick._leftClickSkill._nowCool = 0;
            }
            if (nowTime >= _pick._rSkill._nowCool)
            {
                _pick._rSkill._nowCool = 0;
            }
        }
    }

    public void Init(eCharacter pickChar)
    {
        _maxHp = TableManager._instance.TakeInt(TableManager.eTableJsonNames.PlayerStatus, (int)pickChar, TableManager.ePlayerStatusIndex.HP.ToString());
        _pv.RPC(nameof(PlayerInit), RpcTarget.AllBuffered, pickChar);
        _miniStatus.InitStatus(_maxHp, PhotonNetwork.NickName, Color.green);
        _mainStatusBox = GameObject.Find("StatusSection").GetComponent<MyStatusBox>();
        _mainStatusBox.StatusInit(_att, _def, _speed, _maxHp, pickChar); 
    }

    [PunRPC]
    void PlayerInit(eCharacter pickChar)
    {
        _nowChar = pickChar;
        _pick = GetComponent<Character>();
        _isJump = false;

        _att = TableManager._instance.TakeInt(TableManager.eTableJsonNames.PlayerStatus, (int)pickChar, TableManager.ePlayerStatusIndex.ATT.ToString());
        _def = TableManager._instance.TakeInt(TableManager.eTableJsonNames.PlayerStatus, (int)pickChar, TableManager.ePlayerStatusIndex.DEF.ToString());
        _speed = TableManager._instance.Takefloat(TableManager.eTableJsonNames.PlayerStatus, (int)pickChar, TableManager.ePlayerStatusIndex.Speed.ToString());
        _nowHp = _maxHp = TableManager._instance.TakeInt(TableManager.eTableJsonNames.PlayerStatus, (int)pickChar, TableManager.ePlayerStatusIndex.HP.ToString());

        _pick.InitSkill();
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "Life", (bool)true } });
    }


    public void LeftMouseSkill() { if (_pick._leftClickSkill._nowCool <= 0) Attack((int)eAnimState.MouseLeft); }
    public void RightMouseSkill() { if (_pick._rightClickSkill._nowCool <= 0) Attack((int)eAnimState.MoustRight); }
    public void QSkill() { if (_pick._qSkill._nowCool <= 0) Attack((int)eAnimState.Q); }
    public void ESkill() { if(_pick._eSkill._nowCool <= 0 ) Attack((int)eAnimState.E); }
    public void RSkill() { if (_pick._rSkill._nowCool <= 0) Attack((int)eAnimState.R); }

    public void PlayMove()
    {
        if (_isSkill || _isDeath || _isStuck)
        {
            return;
        }

        if (_isJump)
        {
            int lMask = 1 << LayerMask.NameToLayer("Block");

            if (isGround)
            {
                if (_rigid.velocity.y >= 0)
                {
                    _isJump = false;
                    ChangeAnimState((int)eAnimState.Idle);
                    isGround = false;
                }
            }
            else
            {
                isGround = (_rigid.velocity.y < 0);
            }
            return;
        }
        
        _dir =  new Vector3(-Input.GetAxisRaw("Horizontal"), 0, -Input.GetAxisRaw("Vertical"));

        if (!_isJump && Input.GetButtonDown("Jump"))
        {
            _isJump = true;
            _rigid.AddForce((Vector3.up * _jumpPower + _dir * 1.3f) * _rigid.mass, ForceMode.Impulse);
            ChangeAnimState((int)eAnimState.Jump);
        }
        else if(_dir.magnitude > 0)
        {
            Quaternion rotDir = Quaternion.LookRotation(_dir);
            transform.rotation = rotDir;// Quaternion.Slerp(_rigid.rotation, rotDir, 150 * Time.deltaTime);
            transform.Translate(Vector3.forward * _speed * 2 * Time.deltaTime);
            ChangeAnimState((int)eAnimState.Run);
        }
        else
            ChangeAnimState((int)eAnimState.Idle);
    }

    void Attack(int skill)
    {
        if (_isJump || _isSkill || _isDeath || _isStuck || _isDown)
        {
            return;
        }

        _pv.RPC("RPCAttack", RpcTarget.AllViaServer, skill);
    }
    [PunRPC]
    public void RPCTriggerSwitch(bool isOn, int num)
    {
        _boxTriggers[num].gameObject.SetActive(isOn);
    }

    [PunRPC]
    void RPCAttack(int skill)
    {
        _isSkill = true;
        switch (skill)
        {
            case (int)eAnimState.Q:
                    _nowSkill = _pick._qSkill;
                    if(_pv.IsMine) _mainStatusBox.QSkillCoolTime();
                break;
            case (int)eAnimState.E:
                    _nowSkill = _pick._eSkill;
                    if (_pv.IsMine) _mainStatusBox.ESkillCoolTime();
                break;
            case (int)eAnimState.R:
                    _nowSkill = _pick._rSkill;
                    if (_pv.IsMine) _mainStatusBox.RSkillCoolTime();
                break;
            case (int)eAnimState.MouseLeft:
                    _nowSkill = _pick._leftClickSkill;
                    if (_pv.IsMine) _mainStatusBox.LeftClickSkillCoolTime();
                break;
        }

        if (_isSkill)
        {
            if (_pv.IsMine) _nowSkill._nowCool = _nowSkill._skilStatus._cooltime + PhotonNetwork.Time;
            ChangeAnimState(_nowSkill._skilStatus._animNum, _nowSkill._rNum);
        }
        else
            PlayMove();
    }
    [PunRPC]
    void RPCHit(int hit)
    {
        ChangeAnimState(hit);
        _animator.SetTrigger("HitTime");
    }
    [PunRPC]
    void RPCDead()
    {
        StartCoroutine(Dissolve());
    }
    IEnumerator Dissolve()
    {
        SkinnedMeshRenderer _sr = transform.GetComponentInChildren<SkinnedMeshRenderer>();
        _sr.material = _dissolve;
        while (_sr.materials[0].GetFloat("_Cut") <= 0.3f)
        {
            _sr.materials[0].SetFloat("_Cut", _sr.materials[0].GetFloat("_Cut") + 0.3f * Time.deltaTime);
            yield return null;
        }
        gameObject.SetActive(false);
    }
    public void FGetItem() { }
    public void Evasion() { }

    public void Damaged(stSkill skill, float objAtt)
    {
        if (_isInvincible) return;

        //Debug.LogFormat("AttackDmg : {0} / Coefficient : {1} / ObjAtt : {2} / ", skill._attackDmg, skill._coefficient, objAtt);

        _nowHp -= (skill._attackDmg + skill._coefficient * objAtt) * (_def / (100 + _def));
        //Debug.Log(_nowHp);
        _miniStatus.HPValue(_nowHp);
        if(_pv.IsMine) _mainStatusBox.HPValue(_nowHp);

        _isStuck = true;

        //for (int i = 0; i < (int)eCrowdControl.Cnt; i++)
        //{
        //    Debug.Log(((eCrowdControl)i).ToString() + " : " + skill._isCC[i]);
        //}

        if (_nowHp <= 0)
        {
            // death로 변하고
            ChangeAnimState((int)eAnimState.Down);
            _isDown = true;
            _isDeath = true;
            _animator.SetBool("isDeath", true);
            _animator.SetTrigger("HitTime");
            // death가 끝나면 남아있는 플레이어를 따라가고 해당 오브젝트 삭제
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Life", false } });
            _pv.RPC(nameof(RPCDead), RpcTarget.All);
            MainController._instance.ProgOutView();
        }
        else if (skill._isCC[(int)eCrowdControl.Airborne])
        {
            _rigid.AddForce(Vector3.up * _rigid.mass * 5, ForceMode.Impulse);
            ChangeAnimState((int)eAnimState.Down);
            _isDown = true;
            _animator.SetTrigger("HitTime");
            //Debug.Log("Airbone");
        }
        else if (skill._isCC[(int)eCrowdControl.Down])
        {
            ChangeAnimState((int)eAnimState.Down);
            _isDown = true;
            _animator.SetTrigger("HitTime");
            //Debug.Log("Down");
        }
        else if (skill._isCC[(int)eCrowdControl.Stuck])
        {
            _rigid.AddForce(transform.forward * -0.5f * _rigid.mass, ForceMode.Impulse);
            if(!_isDown) ChangeAnimState((int)eAnimState.Hit);
            _animator.SetTrigger("HitTime");
            //Debug.Log("Stuck");
        }
    }
    public void ChangeAnimState(int state, int rSkill = -1)
    {
        _animator.SetInteger("AnimState", state);
        if(rSkill != -1) _animator.SetInteger("RSkillNumber", rSkill);
    }


    #region [ 애니메이션 함수 ]
    void InvinsibleOn()
    {
        _isInvincible = true;
    }
    void InvinsibleOff()
    {
        _isInvincible = false;
        _isDown = false;
        PlayMove();
    }
    void MovableOff()
    {
        _isStuck = true;
    }
    void MovableOn()
    {
        _isStuck = false;
        _isDown = false;
        ChangeAnimState((int)eAnimState.Idle, -1);
    }
    void AttackOn()
    {
        _nowSkill.StartAttack();
    }
    void AttackOff()
    {
        _nowSkill.EndAttack();
    }
    void MotionEnd()
    {
        _isSkill = false;
        _isDown = false;
        PlayMove();
    }
    #endregion [ 애니메이션 함수 ]

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_nowHp);
            stream.SendNext(_nowChar);
            stream.SendNext(_att);
            stream.SendNext(_isSkill);
        }
        else
        {
            _nowHp = (float)stream.ReceiveNext();
            _nowChar = (eCharacter)stream.ReceiveNext();
            _att = (float)stream.ReceiveNext();
            _isSkill = (bool)stream.ReceiveNext();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("AttackRange"))
        {
            transform.LookAt(new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z));
            Player attackChar = other.transform.parent.parent.GetComponent<Player>();
            stSkill skill = attackChar._nowSkill._skilStatus;
            Damaged(skill, attackChar._att);
        }
        if (other.tag.Equals("Projectile"))
        {
            transform.LookAt(new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z));
            Projectile projectile = other.transform.parent.GetComponent<Projectile>();
            stSkill skill = projectile._mySkill;
            Damaged(skill, projectile._player._att);
        }
    }

    public bool AllhasTag(string key)
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (PhotonNetwork.PlayerList[i].CustomProperties[key] == null) return false;
        }
        return true;
    }
}
