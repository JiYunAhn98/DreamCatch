using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DefineHelper;
public class WindowPopupBtn : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] ePopup popupName;
    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager._instance.OpenPopup(popupName);
    }
}
