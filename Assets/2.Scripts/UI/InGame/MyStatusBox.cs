using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using DefineHelper;
using TMPro;
public class MyStatusBox : MonoBehaviour
{
    [Header("Skill Icons")]
    [SerializeField] SkillIcon _leftClickIcon;
    [SerializeField] SkillIcon _qIcon;
    [SerializeField] SkillIcon _eIcon;
    [SerializeField] SkillIcon _rIcon;
    [SerializeField] SkillIcon _rightClickIcon;

    [Header("Status")]
    [SerializeField] ObjectSpawn _mycharIcon;
    [SerializeField] TextMeshProUGUI _att;
    [SerializeField] TextMeshProUGUI _def;
    [SerializeField] TextMeshProUGUI _speed;

    [Header("HPSlider")]
    [SerializeField] Slider _hpbar;

    public void StatusInit(float att, float def, float speed, float maxHp, eCharacter pickChar)
    {
        _att.text = att.ToString();
        _def.text = def.ToString();
        _speed.text = speed.ToString();

        _hpbar.maxValue = maxHp;
        _hpbar.value = maxHp;

        _mycharIcon.InstanceCharacter(pickChar.ToString());

        _leftClickIcon.InitIcon(pickChar, eSkillIndex.LeftClick);
        _qIcon.InitIcon(pickChar, eSkillIndex.Q);
        _eIcon.InitIcon(pickChar, eSkillIndex.E);
        _rIcon.InitIcon(pickChar, eSkillIndex.R1);
    }
    public void HPValue(float hp)
    {
        _hpbar.value = hp;
    }
    public void QSkillCoolTime()
    {
        _qIcon.CoolTimeSet();
    }
    public void ESkillCoolTime()
    {
        _eIcon.CoolTimeSet();
    }
    public void RSkillCoolTime()
    {
        _rIcon.CoolTimeSet();
    }
    public void LeftClickSkillCoolTime()
    {
        _leftClickIcon.CoolTimeSet();
    }
}
