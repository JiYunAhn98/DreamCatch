using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;
namespace DefineHelper
{
    #region [ Play Game State Enum ]
    public enum eSceneName  // 씬 이름 정리
    {
        Lobby,
        MultyLobbyRoom,
        DreamCatch,

        Cnt
    }
    public enum eRoomProperty
    {
        Title,
        Mode,
        SecretCode,
        MinIndex,
        
        Cnt
    }
    public enum ePlayerProperty
    {
        Character,

        Cnt
    }
    public enum eGameMode   // 게임 모드 이름 정리
    {
        Survival,

        Cnt
    }
    public enum eLobbyPanel
    {
        BGPanel,
        LoginPanel,
        LobbyPanel,
        RoomListPanel,
        InRoomPanel,
        LoadingPanel,
        
        Cnt
    }
    public enum ePlayerReadyState
    {
        Wait,
        SetUp,
        Ready,
        Host,

        Cnt
    }
    public enum eGameState
    {
        Init,
        Ready,
        Play,
        DreamCatch,
        OutView,
        End,

    }
    public enum eAnimState
    {
        Idle,
        Run,
        Jump,
        Q,
        E,
        R,
        F,
        MouseLeft,
        MoustRight,
        Death,
        Hit,
        Down,

        Cnt
    }
    public enum eCrowdControl
    {
        Stuck,
        Down,
        Airborne,

        Cnt
    }
    public enum eRPGWarriorRSkill
    {
        Slash,
        Shield,

        Cnt
    }
    public enum eGameScene
    {
        Lobby,
        DreamCatch,

        Cnt
    }

    #endregion [ Play Object Enum ]

    #region [ Prefab Enum ]
    public enum ePopup      // 팝업 윈도우 이름 정리
    {
        OptionWnd,
        RoomSettingWnd,
        AlarmWnd,
        RoomSearchWnd,
        AccountRegisterWnd,
        InRoomWnd,
        CharacterPickWnd,

        Cnt
    }
    public enum eBlockType
    {
        Simple,
        Special1,
        Special2,
        Special3,
        Line,

        Cnt
    }
    public enum eWizzardBlockType
    {
        Ice,
        Lava,
        Earth,

        Cnt
    }
    public enum eCharacter  // 캐릭터 이름 정리
    {
        RPGWarrior,
        SFSoldier,
        Wizard,

        Cnt
    }
    public enum eSkillIndex
    {
        LeftClick,
        Q,
        E,
        R1,     // R1에서부터 ItemIndex 를 더해주면 된다.
        R2,

        Cnt
    }
    #endregion [ Prefab Enum ]

    #region [ PlayFab ]


    #endregion [ PlayFab ]

    #region [ Struct ]
    /// <summary>
    /// 옵션 설정창에서 설정할 수 있는 데이터 구조체
    /// </summary>
    public struct stOptionData
    {
        public Vector2Int _resolution { get; set; }         // 해상도
        public int _totalVol { get; set; }                  // 전체 음량
        public int _bgmVol { get; set; }                    // bgm 음량
        public int _effectVol { get; set; }                 // 효과음량
        public bool _fullScreen { get; set; }               // 전체화면 여부

        public stOptionData(Vector2Int revol, int totalVol, int bgmVol, int effectVol, bool fullScreen)
        {
            _resolution = revol;
            _totalVol = totalVol;
            _bgmVol = bgmVol;
            _effectVol = effectVol;
            _fullScreen = fullScreen;
        }
        /// <summary>
        /// 옵션창에서 설정 가능한 모든 옵션 내용을 저장한다.
        /// </summary>
        public void SettingAllOption(Vector2Int revol, int totalVol, int bgmVol, int effectVol, bool fullScreen)
        {
            _resolution = revol;
            _totalVol = totalVol;
            _bgmVol = bgmVol;
            _effectVol = effectVol;
            _fullScreen = fullScreen;
        }
    }
    public struct stPlayerData      // 파일에서 불러올 때 초기화
    {
        public string _id { get; set; }
        public string _pw { get; set; }
        public string _nickName { get; set; }
        public int _dp { get; set; }
        public int _cash { get; set; }

        public stPlayerData(string id, string pw, string nickName, int dp, int cash)
        {
            _id = id;
            _pw = pw;
            _nickName = nickName;
            _dp = dp;
            _cash = cash;
        }
    }

    public struct stSkill
    {
        public float _attackDmg;      // 스킬 깡데미지
        public float _coefficient;    // 스킬 계수
        public int _animNum;        // 애니메이션 값
        public float _cooltime;       // 쿨타임

        public bool[] _isCC;       // 경직

        public stSkill(float attackDmg, float coefficient, int animNum, float cool, params int[] cc)
        {
            _attackDmg = attackDmg;
            _coefficient = coefficient;
            _animNum = animNum;
            _cooltime = cool;

            _isCC = new bool[(int)eCrowdControl.Cnt];
            foreach(int ccIndex in cc)
            {
                _isCC[ccIndex] = true;
            }
        }
        public static byte[] Serialize(object customobject)
        {
            stSkill ct = (stSkill)customobject;

            // 스트림에 필요한 메모리 사이즈(Byte)
            MemoryStream ms = new MemoryStream(sizeof(int) + sizeof(int));

            // 각 변수들을 Byte 형식으로 변환, 마지막은 개별 사이즈
            ms.Write(BitConverter.GetBytes(ct._attackDmg), 0, sizeof(int));
            ms.Write(BitConverter.GetBytes(ct._coefficient), 0, sizeof(int));
            ms.Write(BitConverter.GetBytes(ct._animNum), 0, sizeof(int));
            ms.Write(BitConverter.GetBytes(ct._cooltime), 0, sizeof(int));

            foreach (bool tmp in ct._isCC)
            {
                ms.Write(BitConverter.GetBytes(tmp), 0, sizeof(bool));
            }

            // 만들어진 스트림을 배열 형식으로 반환
            return ms.ToArray();
        }

        // 역직렬화
        public static object Deserialize(byte[] bytes)
        {
            stSkill ct = new stSkill();

            // 바이트 배열을 필요한 만큼 자르고, 원하는 자료형으로 변환
            ct._attackDmg = BitConverter.ToInt32(bytes, sizeof(int));
            ct._coefficient = BitConverter.ToInt32(bytes, sizeof(int));
            ct._animNum = BitConverter.ToInt32(bytes, sizeof(int));
            ct._cooltime = BitConverter.ToInt32(bytes, sizeof(int));
            for (int i = 0; i < (int)eCrowdControl.Cnt; i++)
            {
                ct._isCC[i] = BitConverter.ToBoolean(bytes, sizeof(bool));
            }

            return ct;
        }
    }
    #endregion [ Struct ]
}
