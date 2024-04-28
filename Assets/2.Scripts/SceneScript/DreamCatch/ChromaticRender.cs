using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChromaticRender : MonoBehaviour
{
    float _rgbSplit = 0;
    float _chromaticPower = 2;
    public Material _mChromatic;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // ��ũ��Ʈ�� ���͸��� �����ؼ� ������Ƽ �������� ����
        _mChromatic.SetFloat("_RGBSplit", _rgbSplit);
        _mChromatic.SetFloat("_ChromaticPower", _chromaticPower);
        Graphics.Blit(source, destination, _mChromatic);
    }

    public void DreamCatching()
    {
        if(_rgbSplit < 0.1f)  _rgbSplit += 0.005f;
    }
    public void DreamCatchFinish()
    {
        if (_rgbSplit > 0) _rgbSplit -= 0.005f;
    }
}
