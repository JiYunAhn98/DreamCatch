using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Block[,] _blockInforms { get; set; }
    public GameObject _objMap { get; set; }


    public Map(Block[,] blocks, GameObject map)
    {
        _blockInforms = blocks;
        _objMap = map;
    }
}
