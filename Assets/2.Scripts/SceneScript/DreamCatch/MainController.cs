using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DefineHelper;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MainController : MonoSingleton<MainController>
{
    #region [ ���� ]
    //// �ӽ�
    //public GameObject heroPrefab;
    //public Transform respawn;

    // ���� ����
    [SerializeField] MapGenerater _mapGenerator;
    [SerializeField] DreamCatchRender _dreamCatchRender;
    [SerializeField] ChromaticRender _charomaticRender;
    PhotonView _pv;

    // UI ���� ����
    [SerializeField] AlarmBox _alarmBox;
    [SerializeField] TimerBox _timerBox;
    [SerializeField] EventMsgBox _eventMsgBox;
    [SerializeField] MyStatusBox _statusBox;
  

    // ����� ���� ����
    double _startTime;
    double _dpGenTime;
    Vector3 _directLightTurn;

    // ������ ���� ����
    eGameState _nowGameState;
    eCharacter _nowPickCharacter;
    eCharacter _nowDreamCatchCharacter = eCharacter.RPGWarrior;   // PhotonNetwork.LocalPlayer�� �����Ѵ�. ����׿� ���� �ʱ�ȭ
    Vector3 _mySpawnPoint;
    Player _myPlayer;
    CameraWork _camera;
    bool _isAiming;

    // MasterClient ����
    Vector3[] _spawnPoints; // masterclient�� ����


    #endregion [ ���� ]

    #region [ MonoBehaviour Callback ]
    void Start()
    {
        // �ӽ�
        StartCoroutine(ProgInit());
    }

    void Update()
    {
        switch (_nowGameState)
        {
            case eGameState.Init:
                break;
            case eGameState.Ready:
                break;
            case eGameState.Play:

                _timerBox.SetTime(PhotonNetwork.Time - _startTime);
                // Dream Catch�� �� �� 1���� ����� ��� ����
                if (PhotonNetwork.Time - _dpGenTime >= 10)
                {
                    _dpGenTime += 600;
                    _eventMsgBox.EventOccur("DreamCatcher ����!!");
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.InstantiateRoomObject("Prefabs/DreamCatcher", _mapGenerator.GetRandomSafeSpot(), Quaternion.identity);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    _myPlayer.QSkill();
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    _myPlayer.ESkill();
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    _myPlayer.RSkill();
                }
                //else if (Input.GetKeyDown(KeyCode.F))
                //{
                //    _myPlayer.FGetItem();
                //}
                else if (Input.GetMouseButton(0))
                {
                    _myPlayer.LeftMouseSkill();
                }
                //else if (Input.GetMouseButton(1))
                //{
                //    _myPlayer.RightMouseSkill();
                //}
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    _myPlayer.Evasion();
                }
                else
                {
                    _myPlayer.PlayMove();
                }
                if (PhotonNetwork.Time - _startTime >= 600 || GameSet())
                {
                    StartCoroutine(ProgEnd());
                }
                break;
            case eGameState.OutView:

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _camera.ChangeCameraTarget();
                }
                if (PhotonNetwork.Time - _startTime >= 600 || GameSet())
                {
                    StartCoroutine(ProgEnd());
                }
                break;
        }
    }
    #endregion [ MonoBehaviour Callback ]


    #region [ State Pregress ]
    public IEnumerator ProgInit()
    {
        _nowGameState = eGameState.Init;

        CustomTypeRegister.RegistSkillStruct();

        _alarmBox.OpenBox("");

        _pv = GetComponent<PhotonView>();
        _mapGenerator.InitMapGen();
        _nowPickCharacter = (eCharacter)System.Enum.Parse(typeof(eCharacter), PhotonNetwork.LocalPlayer.CustomProperties[ePlayerProperty.Character.ToString()].ToString());

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "GoDreamCatch", true } });
        while (!AllhasTag("GoDreamCatch")) yield return null;

        if (PhotonNetwork.IsMasterClient)
        {
            // �ʸ����
            _mapGenerator.BlockInformInit();                // block init�ؼ� map������ ��������
            _pv.RPC("MakeMap", RpcTarget.MasterClient, (int)_nowDreamCatchCharacter);
            _spawnPoints = new Vector3[PhotonNetwork.CurrentRoom.PlayerCount];

            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                Vector3 tmp = _mapGenerator.GetRandomSafeSpot();
                //Debug.Log(tmp.x + ", " + tmp.y + ", " + tmp.z);

                for (int j = 0; j < i; j++)
                {
                    if (_spawnPoints[j] == tmp)
                    {
                        tmp = _mapGenerator.GetRandomSafeSpot();
                        j = -1;
                        continue;
                    }
                }
                _spawnPoints[i] = tmp;
                PhotonNetwork.PlayerList[i].SetCustomProperties(new Hashtable() { { "mySpawnSpotX", (int)tmp.x }, { "mySpawnSpotZ", (int)tmp.z } });
            }
        }

        // ĳ���� ����, �����ؼ� ���� _pvMine�̾�߸� ������ ����
        while (!AllhasTag("mySpawnSpotX") || !AllhasTag("mySpawnSpotZ")) yield return null;

        _mySpawnPoint = new Vector3( (int)PhotonNetwork.LocalPlayer.CustomProperties["mySpawnSpotX"], 10, (int)PhotonNetwork.LocalPlayer.CustomProperties["mySpawnSpotZ"]);
        string path = "PlayerCharacters/" + _nowPickCharacter.ToString();
        GameObject go = PhotonNetwork.Instantiate(path, _mySpawnPoint, Quaternion.identity);
        _myPlayer = go.GetComponent<Player>();

        // ī�޶� ����
        _camera = Camera.main.gameObject.GetComponent<CameraWork>();
        _camera.OnStartFollowing(_myPlayer.transform);

        // �ʱ�ȭ �۾�
        _directLightTurn = Vector3.right;
        _timerBox.OpenTimerBox(0);
        _myPlayer.Init(_nowPickCharacter);

        StartCoroutine(ProgReady());

        yield break;
    }

    public IEnumerator ProgReady()
    {

        _nowGameState = eGameState.Ready;

        _alarmBox.OpenBox("Ready!!");
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "Life", true } });

        while (!AllhasTag("Life")) yield return null;

        yield return new WaitForSeconds(3.0f);

        _alarmBox.ChangeText("Start!!");

        yield return new WaitForSeconds(1.0f);

        _startTime = PhotonNetwork.Time;
        _dpGenTime = PhotonNetwork.Time;
        _timerBox.OpenTimerBox(PhotonNetwork.Time - _startTime);

        ProgPlay();
    }
    public void ProgPlay()
    {
        _nowGameState = eGameState.Play;
        // �÷��� �� �ߵ��� �͵�
        _alarmBox.CloseBox();
        Time.timeScale = 1;
    }

    public IEnumerator ProgDreamCatch()
    {
        // �帲ĳġ Event �߻�, (�帲ĳġ�� �� �÷��̾� NickName) ���� DreamCatch!! > (���� ���� ĳ���� ����)
        int time = 0;
        double timeTmp = PhotonNetwork.Time;
        _alarmBox.OpenBox("DreamCatch!!");
        _eventMsgBox.EventOccur(PhotonNetwork.LocalPlayer.NickName + "���� DreamCatch!! > " + _nowDreamCatchCharacter.ToString());
        Time.timeScale = 0.01f;

        while (time < 20)   // 10���� ������
        {
            time++;
            _charomaticRender.DreamCatching();
            yield return new WaitForSeconds(0.00025f);
        }
        while (time < 40)   // 10���� ������
        {
            time++;
            _dreamCatchRender.DreamCatching();
            yield return new WaitForSeconds(0.00025f);
        }

        //=============���� �Լ� ����
        // �� ����
        //_nowDreamCatchCharacter = _nowPickCharacter; �帲ĳ�ĸ� ���� ĳ���Ͳ��� �ٲ����
        _pv.RPC("MakeMap", RpcTarget.MasterClient, (int)_nowDreamCatchCharacter);

        while (time > 20)   // 10���� ������
        {
            time--;
            _dreamCatchRender.DreamCatchFinish();
            Debug.Log(time);
            yield return new WaitForSeconds(0.00025f);
        }
        while (time > 0)   // 10���� ������
        {
            time--;
            _charomaticRender.DreamCatchFinish();
            yield return new WaitForSeconds(0.00025f);
        }
        // Game���� �̵�
        _dpGenTime = PhotonNetwork.Time;
        _startTime += _dpGenTime - timeTmp;
        if (_nowGameState == eGameState.Play)
            ProgPlay();
        else ProgOutView();
    }
    public void ProgOutView()
    {
        _nowGameState = eGameState.OutView;
    }
    public IEnumerator ProgEnd()
    {
        _nowGameState = eGameState.End;
        _alarmBox.OpenBox("GAME SET!!");
        yield return new WaitForSeconds(3.0f);
        // ���� ���� �� �Ѱ��� ������ ����
        if(PhotonNetwork.IsMasterClient) _pv.RPC("LoadGameScene", RpcTarget.AllViaServer, eGameScene.Lobby);
    }
    #endregion [ State Pregress ]

    #region [ RPC ]
    //[PunRPC]
    /// <summary>
    /// �ʿ� ���� ������ ��ü�� �ѷ��ش�.
    /// </summary>
    /// <param name="mapInfo"> ���� ���� ���� ���� </param>
    /// <param name="spawnPoints"> ���� ���� ��� �÷��̾��� ��������Ʈ </param>
    //public void GetMapInfo(int[,] noise, int[,] voronoi, Vector2[] spawnPoints)
    //{
    //    _noiseMap = noise;
    //    _voronoiMap = voronoi;
    //    _mySpawnPoint = 
    //}
    //[PunRPC]
    //public void GetMapInfo(int x, int z, int type, int num, int noise)
    //{
    //    _mapInfo.Add(new Vector2Int(x, z), new Block((eBlockType)type, num, noise));
    //}
    [PunRPC]
    public void GetSpawnPoint()
    {
        if (GameObject.FindGameObjectsWithTag("Respawn").Length > 0)
            _mySpawnPoint = GameObject.FindGameObjectsWithTag("Respawn")[SettingManager._instance._myIndex].transform.position;
    }
    [PunRPC]
    /// <summary>
    /// ���� Dream Catch�� ������ ĳ������ ������ �Űܰ���.
    /// </summary>
    /// <param name="dreamCatchChar"> ���� DraemCatch�� ������ ĳ���� ���� </param>
    public void MakeMap(int dreamCatchChar)
    {
        _mapGenerator.CreateBlock((eCharacter)dreamCatchChar);
        _nowDreamCatchCharacter = (eCharacter)dreamCatchChar;
    }
    [PunRPC]
    void LoadGameScene(eGameScene scene)
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        PhotonNetwork.LoadLevel((int)scene);
    }
    [PunRPC]
    void ProgDreamCatchRPC(int dreamCatchChar)
    {
        _nowDreamCatchCharacter = (eCharacter)dreamCatchChar;
        StartCoroutine("ProgDreamCatch");
    }
    #endregion [ RPC ]

    bool GameSet()
    {
        int cnt = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //Debug.Log((bool)PhotonNetwork.PlayerList[i].CustomProperties["Life"]);
            if ((bool)PhotonNetwork.PlayerList[i].CustomProperties["Life"]) cnt++;
        }
        //Debug.Log(cnt);
        return (cnt <= 1);
    }
    public bool AllhasTag(string key)
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (PhotonNetwork.PlayerList[i].CustomProperties[key] == null) return false;
        }
        return true;
    }
    //private void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 30;
    //    style.normal.textColor = Color.black;
    //    if (GUI.Button(new Rect(0, 0, 200, 200), "rpgwarrior"))
    //    {
    //        _pv.RPC("ProgDreamCatchRPC", RpcTarget.AllViaServer, (int)eCharacter.RPGWarrior);
    //    }
    //    if (GUI.Button(new Rect(0, 220, 200, 200), "SFSoldier"))
    //    {
    //        _pv.RPC("ProgDreamCatchRPC", RpcTarget.AllViaServer, (int)eCharacter.SFSoldier);
    //    }
    //}
}
