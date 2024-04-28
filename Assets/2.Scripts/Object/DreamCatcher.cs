using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DefineHelper;

public class DreamCatcher : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.up * 0.3f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            MainController._instance.GetComponent<PhotonView>().RPC("ProgDreamCatchRPC", RpcTarget.AllViaServer, (int)other.gameObject.GetComponent<Character>()._pickChar);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
