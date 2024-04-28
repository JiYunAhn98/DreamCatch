using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class Block
{
    public eBlockType _type { get; set; }
    public int _originNum { get; set;}
    public int _noiseHeight { get; set; }


    public Block(eBlockType type, int num, int noiseHeight)
    {
        _type = type;
        _originNum = num;
        _noiseHeight = noiseHeight;
    }
}
