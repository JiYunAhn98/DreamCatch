using csDelaunay;   //Delaunay의 voronoi를 사용하기위함
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DefineHelper;
using System.Linq;
using System.IO;
using Photon.Pun;

/// <summary>
/// 100, 5, 40 짜리맵을 만든다.
/// 노이즈 맵은 5칸 짜리 Height를 결정해주는 역할
/// 보로노이 다이어그램은 각 영역에서 나올 지형을 선택해주는 역할
/// </summary>
public class MapGenerater : MonoBehaviour
{
    // 맵을 만들기 위한 Texture정보를 저장
    [Header("맵 정보")]
    [SerializeField] Transform _mapRoot;
    [SerializeField] int _mapMaxHeight = 5;
    [SerializeField] int _percent = 1;
    [SerializeField] Vector2Int _mapSize;                      // 맵의 사이즈

    [Header("노이즈 맵 정보")]
    [SerializeField] FastNoiseLite.NoiseType _noiseType;
    [SerializeField] FastNoiseLite.FractalType _fractalType;
    [SerializeField, Range(0f, 0.4f)] float _noiseFrequency = 0.07f;
    [SerializeField] int _noiseOctave = 10;

    [Header("바이옴 맵 정보")]
    [SerializeField] int _nodeAmount = 20;               // 중심점의 노드갯수
    [SerializeField] int _loydIterateCount = 3;         // 보로노이 횟수

    // 맵 생성에 따른 정보
    int[,] _voronoiMap;                                 // 맵정보가 저장된 보로노이 다이어그램
    int[,] _noiseMap;                                   // 맵정보가 저장된 노이즈맵
    Dictionary<Vector2Int, Block> _mapInfo;             // Dictionary<(x, z), voronoi와 noise에 따른 블럭의 특성>
    Dictionary<eCharacter,GameObject> _dreamcatchMapObj;// 실제 생성한 맵 전체가 저장되어 있는 집합의 Dictionary
    eCharacter _mapCharacter;
    Vector3[] _spawnPoints;                 // 플레이어 전체가 spawn되기 위해서 저장해둔 포인트
    GameObject _nowMap;
    PhotonView _pv;
    BoxCollider _killZone;

    #region [ 외부 함수 ]
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
    /// 만들어낼 블럭의 Info를 초기화 한다.
    /// </summary>
    /// <returns> 초기화한 블럭의 정보 Dictionary<(x, z), voronoi와 noise에 따른 블럭의 특성> </returns>
    public void BlockInformInit()
    {
        // 노이즈맵을 생성
        CreateNoiseMap(_mapSize, _noiseFrequency, _noiseOctave);    // 노이즈맵정보와.
        //noiseMap = _noiseMap;
        // 보로노이 다이어그램을 생성
        CreateVoronoi(_mapSize, _nodeAmount, _loydIterateCount);    // 보로노이정보를 그대로 넘겨주고
        //voronoiMap = _voronoiMap;
        // 모든 내용을 통틀어서 맵의 정보를 총합
        CreateMapInfo();     // 합치는 작업을 자신이 직접 하자
    }

    /// <summary>
    /// 보로노이는 맵의 블럭 정보, 노이즈맵은 블럭의 높이 정보
    /// 프리팹은 파일이기 때문에 불러오지 않으면 데이터에 누적되지 않기 때문에 파일로 저장한다.
    /// 옥트리방식으로 캐릭터가 있는 부분부터 보이게 해서 로딩시간의 문제를 해결한다.
    /// </summary>
    /// <param name="player"> 맵을 만들어주기 위한 캐릭터의 정보 </param>
    /// <param name="mapInfo"> 현재 방의 맵정보 </param>
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

                    // 보로노이 영역에 해당하는 mapifo와 Simple 중 확률에 따라 선택
                    eBlockType voronoiArea = Random.Range(0, 100) < _percent ? _mapInfo[pos]._type : eBlockType.Simple;

                    // 현재 프리팹에 있는 무작위 숫자의 Object를 가져온다. 현재는 1개 밖에 없음, 파일로 받아올 것
                    int prefabIndex = 1; // Random.Range(1, 2);

                    // 정보를 바탕으로 만들어 낸다.
                    path = "Cubes/" + (curPlayer).ToString() + "/" + voronoiArea.ToString() + "_" + prefabIndex.ToString();
                    _pv.RPC("BlockInstance", RpcTarget.All, path, x, _noiseMap[x, z], z);
                }
            }
            // 다 만들어낸 맵을 player 종류에 따라서 저장
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
    /// 만들어낸 Block의 정보들에서 Simple인 부분만을 골라 return
    /// </summary>
    /// <param name="mapInfo"> 판단하기 위한 mapinfo </param>
    /// <returns> 블럭 type이 Simple인 랜덤한 위치 </returns>
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
    #endregion [ 외부 함수 ]


    #region [ 내부 함수 ]
    public void CreateMapInfo()
    {
        // Dictionary<(x, z), voronoi와 noise에 따른 블럭의 특성>

        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int z = 0; z < _mapSize.y; z++)
            {
                // x와 z의 좌표에 보로노이에 해당하는 값 or Simple로 좌표의 객체에 정체성 부여
                int type = Random.Range(0, 100) < _percent ? _voronoiMap[x, z] : (int)eBlockType.Simple;
                int prefabIndex = Random.Range(1, 2);           // 현재 프리팹에 있는 무작위 숫자의 Object를 가져온다.

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

        //색은 0~1범위로 0은 검은색, 1은 흰색을 나타낸다.
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                float noiseFactor = noise.GetNoise(x, y);       // 무작위 수
                _noiseMap[x,y] = (int)(noiseFactor * (_mapMaxHeight - 1));
            }
        }
        ShowNoiseMap();
    }
    void CreateVoronoi(Vector2Int size, int nodeAmount, int loydCount)
    {
        // 처음에는 랜덤하게 찍어놓고 각 시작점에서 델루네의 삼각형으로 중심점을 옮긴다.

        // 맵 각 지역의 대표값 좌표를 랜덤하게 찍음
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

        //  보로노이, 둘로네 삼각분할을 유전적알고리즘을 통해서 실행
        Rect rect = new Rect(0, 0, size.x, size.y);
        Voronoi vo = new Voronoi(centroids, rect, loydCount);


        // 무게중심그리기(점찍기)
        List<Vector2> siteCoords = vo.SiteCoords(); // 무게중심의 위치
        int[] coordsVal = new int[nodeAmount];      // 무게중심의 값 배열

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

        // 가장자리 그리기(모서리 그리기)
        foreach (Site site in vo.Sites)
        {
            List<Site> neighbors = site.NeighborSites();
            // 이웃한 폴리곤들에게서 겹치는 가장자리를 유도 => 삼각분할
            foreach (Site neightbor in neighbors)
            {
                csDelaunay.Edge edge = vo.FindEdgeFromAdjacentPolygons(site, neightbor);

                if (edge.ClippedVertices is null)
                {
                    continue;
                }
                // 가장자리를 이루는 모서리 정점 2개
                Vector2 corner1 = edge.ClippedVertices[LR.LEFT];
                Vector2 corner2 = edge.ClippedVertices[LR.RIGHT];

                // 1차함수 그래프를 그리듯 가장자리선분을 그린다.
                Vector2 targetPoint = corner1;
                float delta = 1 / (corner2 - corner1).magnitude;
                float lerpRatio = 0f;
                while ((int)targetPoint.x != (int)corner2.x || (int)targetPoint.y != (int)corner2.y)
                {
                    // corner1하고 corner2 사이의 점을 선형보간을 통해 lerpRatio만큼나누는 점을 받아온다.
                    targetPoint = Vector2.Lerp(corner1, corner2, lerpRatio);
                    lerpRatio += delta;

                    // 텍스쳐의 좌표 영역은 0~size.x-1 이지만 생성한 보로노이 다이어그램의 좌표는 ~(float)size.x이다.
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
            int cy = Mathf.RoundToInt(coord.y)+1;       // 정확하게 자기자신을 가리키면 원하는 인덱스와 같으므로 맵을 만들지 않는다.
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
                val = (val + 1) * 0.5f; //0~1이 된다.
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
    #endregion [ 내부 함수 ]
}
