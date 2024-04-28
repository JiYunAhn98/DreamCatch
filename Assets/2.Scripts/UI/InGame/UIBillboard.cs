using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    Transform _tfCam;
    private void Start()
    {
        _tfCam = Camera.main.transform;
    }
    void Update()
    {
        transform.LookAt( new Vector3(transform.position.x, _tfCam.position.y, transform.position.z));
    }
}
