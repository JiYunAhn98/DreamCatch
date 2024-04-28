using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using Photon.Pun;
using Photon.Realtime;


public class ObjectSpawn : MonoBehaviour
{
    Dictionary<eCharacter, GameObject> _characters;
    public eCharacter _nowPickChar { get; set; }
    private void Awake()
    {
        _characters = new Dictionary<eCharacter, GameObject>();
    }

    public void InstanceCharacter(eCharacter charName)
    {
        HideCharacter();
        _nowPickChar = charName;

        if (_characters.ContainsKey(charName))
        {
            _characters[_nowPickChar].SetActive(true);
        }
        else
        {
            GameObject go = Instantiate(Resources.Load("PlayerCharacters/" + charName.ToString() + "Lobby"), transform) as GameObject;
            _characters.Add(charName, go);
        }
    }

    public void InstanceCharacter(string charNameStr)
    {
        eCharacter charName = (eCharacter)System.Enum.Parse(typeof(eCharacter), charNameStr);

        HideCharacter();
        _nowPickChar = charName;

        if (_characters.ContainsKey(charName))
        {
            _characters[_nowPickChar].SetActive(true);
        }
        else
        {
            GameObject go = Instantiate(Resources.Load("PlayerCharacters/" + charName.ToString() + "Lobby"), transform) as GameObject;
            _characters.Add(charName, go);
        }
    }


    public void HideCharacter()
    {
        if (_characters.ContainsKey(_nowPickChar))
            _characters[_nowPickChar].SetActive(false);
    }

}
