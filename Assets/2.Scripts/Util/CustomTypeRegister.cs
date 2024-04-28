using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

using ExitGames.Client.Photon;

public static class CustomTypeRegister
{
    public static void RegistSkillStruct()
    {
        PhotonPeer.RegisterType(typeof(stSkill), (byte)'W', stSkill.Serialize, stSkill.Deserialize);
    }
}
