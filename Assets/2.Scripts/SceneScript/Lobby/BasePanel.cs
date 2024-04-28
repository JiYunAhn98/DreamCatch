using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public abstract class BasePanel : MonoBehaviourPunCallbacks
{
    public abstract void MyTurnInit();
}
