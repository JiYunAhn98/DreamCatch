using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class CharacterFactory : MonoBehaviour
{
    public Player Create(eCharacter name)
    {
        Player character = new Player();

        switch (name)
        {
            case eCharacter.RPGWarrior:
                break;
            case eCharacter.SFSoldier:
                break;
            default:
                break;
        }

        return character;
    }
}
