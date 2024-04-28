using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DefineHelper;

public class LobbyCharacterRotate : MonoBehaviour, IDragHandler
{
    [SerializeField] GameObject _character;
    public float speed = 5f;
    Vector3 _rot;

    void Start()
    {
        _rot = _character.transform.localRotation.eulerAngles;
    }
    public void OnDrag(PointerEventData eventData)
    {
        _rot.y -= Input.GetAxis("Mouse X") * speed;
        _character.transform.localEulerAngles = _rot;
    }
}
