using csDelaunay;   //Delaunay�� voronoi�� ����ϱ�����
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DefineHelper;
using System.Linq;
using System.IO;
using Photon.Pun;

/// <summary>
/// 100, 5, 40 ¥������ �����.
/// ������ ���� 5ĭ ¥�� Height�� �������ִ� ����
/// ���γ��� ���̾�׷��� �� �������� ���� ������ �������ִ� ����
/// </summary>
public class MapGenerater : MonoBehaviour
{
    // ���� ����� ���� Texture������ ����
    [Header("�� ����")]
    [SerializeField] Transform _mapRoot;
    [SerializeField] int _mapMaxHeight = 5;
    [SerializeField] int _percent = 1;
    [SerializeField] Vector2Int _mapSize;                      // ���� ������

    [Header("������ �� ����")]
    [SerializeField] FastNoiseLite.NoiseType _noiseType;
    [SerializeField] FastNoiseLite.FractalType _fractalType;
    [SerializeField, Range(0f, 0.4f)] float _noiseFrequency = 0.07f;
    [SerializeField] int _noiseOctave = 10;

    [Header("���̿� �� ����")]
    [SerializeField] int _nodeAmount = 20;               // �߽����� ��尹��
    [SerializeField] int _loydIterateCount = 3;         // ���γ��� Ƚ��

    // �� ������ ���� ����
    int[,] _voronoiMap;                                 // �������� ����� ���γ��� ���̾�׷�
    int[,] _noiseMap;                                   // �������� ����� �������
    Dictionary<Vector2Int, Block> _mapInfo;             // Dictionary<(x, z), voronoi�� noise�� ���� ���� Ư��>
    Dictionary<eCharacter,GameObject> _dreamcatchMapObj;// ���� ������ �� ��ü�� ����Ǿ� �ִ� ������ Dictionary
    eCharacter _mapCharacter;
    Vector3[] _spawnPoints;                 // �÷��̾� ��ü�� spawn�Ǳ� ���ؼ� �����ص� ����Ʈ
    GameObject _nowMap;
    PhotonView _pv;
    BoxCollider _killZone;

    #region [ �ܺ� �Լ� ]
    public void InitMapGen()
    {
        _dreamcatchMapObj = new Dictionary<eCharacter, GameObject>();
        _mapInfo = new Dictionary<Vector2Int, Block>();
        _pv = GetComponent<PhotonView>();
        _noiseMap = new int[_mapSize.x, _mapSize.y];
        _voronoiMap = new int[_mapSize.x, _mapSize.y];
        _pv.RPC(nameof(SetKillZoneArea), RpcTarget.AllBuffered, _mapSize.x, _mapSize.y);
    }
    [PunRPC]
    public void SetKillZoneArea(int x, int y)
    {
        _killZone = GetComponentInChildren<BoxCollider>();
        int killzoneX = x + 10, killzoneY = (_mapMaxHeight / 2) - 5, killzoneZ = y + 10;
        _killZone.size = new Vector3(killzoneX, 2, killzoneZ);
        _killZone.center = new Vector3(killzoneX/2 - 5, killzoneY, killzoneZ/2 - 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().Damaged(new stSkill(int.MaxValue, 1, 0, 0), 1);
        }
    }
    /// <summary>
    /// ���� ���� Info�� �ʱ�ȭ �Ѵ�.
    /// </summary>
    /// <returns> �ʱ�ȭ�� ���� ���� Dictionary<(x, z), voronoi�� noise�� ���� ���� Ư��> </returns>
    public void BlockInformInit()
    {
        // ��������� ����
        CreateNoiseMap(_mapSize, _noiseFrequency, _noiseOctave);    // �������������.
        //noiseMap = _noiseMap;
        // ���γ��� ���̾�׷��� ����
        CreateVoronoi(_mapSize, _nodeAmount, _loydIterateCount);    // ���γ��������� �״�� �Ѱ��ְ�
        //voronoiMap = _voronoiMap;
        // ��� ������ ��Ʋ� ���� ������ ����
        CreateMapInfo();     // ��ġ�� �۾��� �ڽ��� ���� ����
    }

    /// <summary>
    /// ���γ��̴� ���� �� ����, ��������� ���� ���� ����
    /// �������� �����̱� ������ �ҷ����� ������ �����Ϳ� �������� �ʱ� ������ ���Ϸ� �����Ѵ�.
    /// ��Ʈ��������� ĳ���Ͱ� �ִ� �κк��� ���̰� �ؼ� �ε��ð��� ������ �ذ��Ѵ�.
    /// </summary>
    /// <param name="player"> ���� ������ֱ� ���� ĳ������ ���� </param>
    /// <param name="mapInfo"> ���� ���� ������ </param>
    public void CreateBlock(eCharacter curPlayer)
    {
        if (_dreamcatchMapObj.ContainsKey(_mapCharacter))
            _pv.RPC("MapOff", RpcTarget.All, (int)_mapCharacter);

        if (!_dreamcatchMapObj.ContainsKey(curPlayer))
        {
            string path = "Prefabs/MapParent";
            GameObject go = PhotonNetwork.Instantiate(path, Vector3.zero, Quaternion.identity);

            Vector2Int pos = new Vector2Int(0, 0);
            for (int x = 0; x < _mapSize.x; x++)
            {
                for (int z = 0; z < _mapSize.y; z++)
                {
                    pos.x = x;
                    pos.y = z;

                    // ���γ��� ������ �ش��ϴ� mapifo�� Simple �� Ȯ���� ���� ����
                    eBlockType voronoiArea = Random.Range(0, 100) < _percent ? _mapInfo[pos]._type : eBlockType.Simple;

                    // ���� �����տ� �ִ� ������ ������ Object�� �����´�. ����� 1�� �ۿ� ����, ���Ϸ� �޾ƿ� ��
                    int prefabIndex = 1; // Random.Range(1, 2);

                    // ������ �������� ����� ����.
                    path = "Cubes/" + (curPlayer).ToString() + "/" + voronoiArea.ToString() + "_" + prefabIndex.ToString();
                    _pv.RPC("BlockInstance", RpcTarget.All, path, x, _noiseMap[x, z], z);
                }
            }
            // �� ���� ���� player ������ ���� ����
            path = "Prefabs/Point";
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                Vector3 point = GetRandomSafeSpot();
                PhotonNetwork.Instantiate(path, point, Quaternion.identity);
            }
            _pv.RPC("MapMergeParent", RpcTarget.All, (int)curPlayer, go.tag, "FloorBox");
        }
        else
        {
            _pv.RPC("MapOn", RpcTarget.All, (int)curPlayer);
        }
        _mapCharacter = curPlayer;
    }
    [PunRPC]
    void BlockInstance(string path, int x, int height, int z)
    {
        GameObject prefab = (GameObject)Resources.Load(path);
        Instantiate(prefab, new Vector3(x, height, z), Quaternion.identity);
    }
    [PunRPC]
    void MapMergeParent(int nowChar, string parentTag, string childTag)
    {
        Transform parent = GameObject.FindGameObjectWithTag(parentTag).GetComponent<Transform>();
        GameObject[] blocks = GameObject.FindGameObjectsWithTag(childTag);
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].gameObject.transform.SetParent(parent);
        }
        _dreamcatchMapObj.Add((eCharacter)nowChar, parent.gameObject);
    }
    [PunRPC]
    void MapOn(int nowChar)
    {
        _dreamcatchMapObj[(eCharacter)nowChar].SetActive(true);
    }
    [PunRPC]
    void MapOff(int prevChar)
    {
        _dreamcatchMapObj[(eCharacter)prevChar].SetActive(false);
    }
    /// <summary>
    /// ���� Block�� �����鿡�� Simple�� �κи��� ��� return
    /// </summary>
    /// <param name="mapInfo"> �Ǵ��ϱ� ���� mapinfo </param>
    /// <returns> �� type�� Simple�� ������ ��ġ </returns>
    public Vector3 GetRandomSafeSpot()
    {
        Vector2Int pos = new Vector2Int(Random.Range(0, _mapSize.x), Random.Range(0, _mapSize.y));
        while (true)
        {
            if (_mapInfo[pos]._type == eBlockType.Simple)
            {
                break;
            }
            else
            {
                pos.x = Random.Range(0, _mapSize.x);
                pos.y = Random.Range(0, _mapSize.y);
            }
        }
        return new Vector3(pos.x, 10, pos.y);
    }
    #endregion [ �ܺ� �Լ� ]


    #region [ ���� �Լ� ]
    public void CreateMapInfo()
    {
        // Dictionary<(x, z), voronoi�� noise�� ���� ���� Ư��>

        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int z = 0; z < _mapSize.y; z++)
            {
                // x�� z�� ��ǥ�� ���γ��̿� �ش��ϴ� �� or Simple�� ��ǥ�� ��ü�� ��ü�� �ο�
                int type = Random.Range(0, 100) < _percent ? _voronoiMap[x, z] : (int)eBlockType.Simple;
                int prefabIndex = Random.Range(1, 2);           // ���� �����տ� �ִ� ������ ������ Object�� �����´�.

                _mapInfo.Add(new Vector2Int(x, z), new Block((eBlockType)type, prefabIndex, _noiseMap[x, z]));
            }
        }
    }
    void CreateNoiseMap(Vector2Int size, float frequency, int octave)
    {
        int seed = Random.Range(1, int.MaxValue);

        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(_noiseType);
        noise.SetFractalType(_fractalType);
        noise.SetFrequency(frequency);
        noise.SetFractalOctaves(octave);
        noise.SetSeed(seed);

        //���� 0~1������ 0�� ������, 1�� ����� ��Ÿ����.
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                float noiseFactor = noise.GetNoise(x, y);       // ������ ��
                _noiseMap[x,y] = (int)(noiseFactor * (_mapMaxHeight - 1));
            }
        }
        ShowNoiseMap();
    }
    void CreateVoronoi(Vector2Int size, int nodeAmount, int loydCount)
    {
        // ó������ �����ϰ� ������ �� ���������� ������� �ﰢ������ �߽����� �ű��.

        // �� �� ������ ��ǥ�� ��ǥ�� �����ϰ� ����
        List<Vector2> centroids = new List<Vector2>();
                
        for (int n = 0; n < nodeAmount; n++)
        {
            int rx = Random.Range(0, _mapSize.x);
            int ry = Random.Range(0, _mapSize.y);

            centroids.Add(new Vector2Int(rx, ry));
            _voronoiMap[rx, ry] = 1;
        }
        ShowVoronoiMap(0);
        for (int n = 0; n < nodeAmount; n++)
        {
            _voronoiMap[(int)centroids[n].x, (int)centroids[n].y] = 0;
        }

        //  ���γ���, �ѷγ� �ﰢ������ �������˰����� ���ؼ� ����
        Rect rect = new Rect(0, 0, size.x, size.y);
        Voronoi vo = new Voronoi(centroids, rect, loydCount);


        // �����߽ɱ׸���(�����)
        List<Vector2> siteCoords = vo.SiteCoords(); // �����߽��� ��ġ
        int[] coordsVal = new int[nodeAmount];      // �����߽��� �� �迭

        int num = 0;
        foreach (Vector2 coord in siteCoords)
        {
            int x = Mathf.RoundToInt(coord.x);
            int y = Mathf.RoundToInt(coord.y);
            _voronoiMap[x, y] = 1;
        }
        ShowVoronoiMap(1);
        foreach (Vector2 coord in siteCoords)
        {
            int x = Mathf.RoundToInt(coord.x);
            int y = Mathf.RoundToInt(coord.y);
            _voronoiMap[x, y] = 0;
        }

        foreach (Vector2 coord in siteCoords)
        {
            int x = Mathf.RoundToInt(coord.x);
            int y = Mathf.RoundToInt(coord.y);
            _voronoiMap[x, y] = Random.Range(0, (int)eBlockType.Line);
            coordsVal[num++] = _voronoiMap[x, y];
        }
        ShowVoronoiMap(2);

        // �����ڸ� �׸���(�𼭸� �׸���)
        foreach (Site site in vo.Sites)
        {
            List<Site> neighbors = site.NeighborSites();
            // �̿��� ������鿡�Լ� ��ġ�� �����ڸ��� ���� => �ﰢ����
            foreach (Site neightbor in neighbors)
            {
                csDelaunay.Edge edge = vo.FindEdgeFromAdjacentPolygons(site, neightbor);

                if (edge.ClippedVertices is null)
                {
                    continue;
                }
                // �����ڸ��� �̷�� �𼭸� ���� 2��
                Vector2 corner1 = edge.ClippedVertices[LR.LEFT];
                Vector2 corner2 = edge.ClippedVertices[LR.RIGHT];

                // 1���Լ� �׷����� �׸��� �����ڸ������� �׸���.
                Vector2 targetPoint = corner1;
                float delta = 1 / (corner2 - corner1).magnitude;
                float lerpRatio = 0f;
                while ((int)targetPoint.x != (int)corner2.x || (int)targetPoint.y != (int)corner2.y)
                {
                    // corner1�ϰ� corner2 ������ ���� ���������� ���� lerpRatio��ŭ������ ���� �޾ƿ´�.
                    targetPoint = Vector2.Lerp(corner1, corner2, lerpRatio);
                    lerpRatio += delta;

                    // �ؽ����� ��ǥ ������ 0~size.x-1 ������ ������ ���γ��� ���̾�׷��� ��ǥ�� ~(float)size.x�̴�.
                    int x = Mathf.Clamp((int)targetPoint.x, 0, size.x - 1);
                    int y = Mathf.Clamp((int)targetPoint.y, 0, size.y - 1);

                    _voronoiMap[x, y] = (int)eBlockType.Line;
                }
            }
        }
        ShowVoronoiMap(3);

        num = 0;
        foreach (Vector2 coord in siteCoords)
        {
            int cx = Mathf.RoundToInt(coord.x);
            int cy = Mathf.RoundToInt(coord.y)+1;       // ��Ȯ�ϰ� �ڱ��ڽ��� ����Ű�� ���ϴ� �ε����� �����Ƿ� ���� ������ �ʴ´�.
            ColorringFace(_voronoiMap, cx, cy, (int)eBlockType.Line, coordsVal[num++], size);
        }
        ShowVoronoiMap(4);
    }
    void ColorringFace(int[,] map, int x, int y, int checknum, int targetNum, Vector2Int size)
    {
        if (x >= size.x || x < 0 || y >= size.y || y < 0) return;
        if (map[x, y] == checknum) return;
        if (map[x, y] == targetNum) return;
        map[x, y] = targetNum;

        ColorringFace(map, x - 1, y, checknum, targetNum, size);
        ColorringFace(map, x + 1, y, checknum, targetNum, size);
        ColorringFace(map, x, y - 1, checknum, targetNum, size);
        ColorringFace(map, x, y + 1, checknum, targetNum, size);
    }
    void ShowNoiseMap()
    {
        Color[] colors = new Color[_noiseMap.Length];
        for (int z = 0; z < _noiseMap.GetLength(1); z++)
        {
            for (int x = 0; x < _noiseMap.GetLength(0); x++)
            {
                float val = _noiseMap[x,z] / (float)(_mapMaxHeight);
                val = (val + 1) * 0.5f; //0~1�� �ȴ�.
                colors[x + _noiseMap.GetLength(0) * z ] = new Color(val, val, val, 1);
            }
        }
        DrawSprite(_mapSize, colors, "noisemap");
    }
    void ShowVoronoiMap(int num)
    {
        Color[] colors = new Color[_voronoiMap.Length];
        for (int z = 0; z < _noiseMap.GetLength(1); z++)
        {
            for (int x = 0; x < _noiseMap.GetLength(0); x++)
            {
                float val = _voronoiMap[x, z];
                Color myCol = Color.white;
                switch (val)
                {
                    case 0: myCol = Color.black;
                        break;
                    case 1:
                        myCol = Color.red;
                        break;
                    case 2:
                        myCol = Color.yellow;
                        break;
                    case 3:
                        myCol = Color.blue;
                        break;
                }

                colors[x + _noiseMap.GetLength(0) * z] = myCol;
            }
        }
        DrawSprite(_mapSize, colors, "voronoiMap" + num);
    }
    Sprite DrawSprite(Vector2Int size, Color[] colordatas, string saveFileName)
    {
        Texture2D texture = new Texture2D(size.x, size.y);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colordatas);
        texture.Apply();

        CreateFileFormTexture2D(texture, saveFileName);

        Rect rect = new Rect(0, 0, size.x, size.y);
        Sprite map = Sprite.Create(texture, rect, Vector2.one * 0.5f);
        return map;
    }
    void CreateFileFormTexture2D(Texture2D texture, string saveFileName)
    {
        byte[] by = texture.EncodeToPNG();
#if UNITY_EDITOR
        string path = Application.dataPath + "/998.texture/" + saveFileName + ".png";
#else
        string path = Application.dataPath +  saveFileName + ".png";
#endif

        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(by);
        bw.Close();
        fs.Close();
    }
    #endregion [ ���� �Լ� ]
}
