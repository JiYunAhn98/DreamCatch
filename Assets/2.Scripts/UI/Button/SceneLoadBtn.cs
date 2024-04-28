using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DefineHelper;

public class SceneLoadBtn : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] eSceneName _nextScene;
    public void OnPointerClick(PointerEventData eventData) 
    {
        SceneControllManager._instance.LoadScene(_nextScene);
    }
}
