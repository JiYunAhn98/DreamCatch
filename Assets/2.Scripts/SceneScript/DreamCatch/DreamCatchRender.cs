using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamCatchRender : MonoBehaviour
{
    float _shadow = 0;
    Color _shadowCol = Color.black;
    public Material _mdreamCatch;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // ��ũ��Ʈ�� ���͸��� �����ؼ� ������Ƽ �������� ����
        _mdreamCatch.SetFloat("_ShadowThreshold", _shadow);
        _mdreamCatch.SetColor("_ShadowColor", _shadowCol);
        Graphics.Blit(source, destination, _mdreamCatch);
    }
    public void DreamCatching()
    {
        if (_shadow < 1f) _shadow += 0.05f;
    }
    public void DreamCatchFinish()
    {
        if (_shadow > 0) _shadow -= 0.05f;
    }
}
