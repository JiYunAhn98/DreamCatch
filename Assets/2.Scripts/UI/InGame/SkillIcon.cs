using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefineHelper;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
public class SkillIcon : MonoBehaviour
{
    [SerializeField] Image _coolTimeBlock;
    [SerializeField] Image _Icon;
    [SerializeField] TextMeshProUGUI _coolText;

    double _nowCoolTime;
    float _coolTime;

    public void InitIcon(eCharacter pickChar, eSkillIndex key)
    {
        string path = "SkillIcon/" + pickChar.ToString() + "Skill/" + pickChar.ToString() +"SkillIcon";
        string name = TableManager._instance.TakeString((TableManager.eTableJsonNames)(pickChar + 1), (int)key, TableManager.eSkillIndex.ResourceName.ToString());

        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        foreach (Sprite sp in sprites)
            if (sp.name == name)
                _Icon.sprite = sp;

        _coolTime = TableManager._instance.Takefloat((TableManager.eTableJsonNames)(pickChar + 1), (int)key, TableManager.eSkillIndex.CoolTime.ToString());
        _coolTimeBlock.gameObject.SetActive(false);
    }

    public void CoolTimeSet()
    {
        _coolTimeBlock.fillAmount = 1;
        _nowCoolTime = PhotonNetwork.Time;
        _coolTimeBlock.gameObject.SetActive(true);
        StartCoroutine("SkillCoolTime");
    }

    IEnumerator SkillCoolTime()
    {
        while (_coolTimeBlock.fillAmount > 0)
        {
            _coolTimeBlock.fillAmount = 1 - (float)(PhotonNetwork.Time - _nowCoolTime) / _coolTime;
            _coolText.text = ((int)(_coolTime - (PhotonNetwork.Time - _nowCoolTime))).ToString();
            yield return new WaitForEndOfFrame();
        }
        _coolTimeBlock.gameObject.SetActive(false);
        yield break;
    }

}
