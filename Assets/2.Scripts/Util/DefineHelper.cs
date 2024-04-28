using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;
namespace DefineHelper
{
    #region [ Play Game State Enum ]
    public enum eSceneName  // �� �̸� ����
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
    public enum eGameMode   // ���� ��� �̸� ����
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
    public enum ePopup      // �˾� ������ �̸� ����
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
    public enum eCharacter  // ĳ���� �̸� ����
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
        R1,     // R1�������� ItemIndex �� �����ָ� �ȴ�.
        R2,

        Cnt
    }
    #endregion [ Prefab Enum ]

    #region [ PlayFab ]


    #endregion [ PlayFab ]

    #region [ Struct ]
    /// <summary>
    /// �ɼ� ����â���� ������ �� �ִ� ������ ����ü
    /// </summary>
    public struct stOptionData
    {
        public Vector2Int _resolution { get; set; }         // �ػ�
        public int _totalVol { get; set; }                  // ��ü ����
        public int _bgmVol { get; set; }                    // bgm ����
        public int _effectVol { get; set; }                 // ȿ������
        public bool _fullScreen { get; set; }               // ��üȭ�� ����

        public stOptionData(Vector2Int revol, int totalVol, int bgmVol, int effectVol, bool fullScreen)
        {
            _resolution = revol;
            _totalVol = totalVol;
            _bgmVol = bgmVol;
            _effectVol = effectVol;
            _fullScreen = fullScreen;
        }
        /// <summary>
        /// �ɼ�â���� ���� ������ ��� �ɼ� ������ �����Ѵ�.
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
    public struct stPlayerData      // ���Ͽ��� �ҷ��� �� �ʱ�ȭ
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
        public float _attackDmg;      // ��ų ��������
        public float _coefficient;    // ��ų ���
        public int _animNum;        // �ִϸ��̼� ��
        public float _cooltime;       // ��Ÿ��

        public bool[] _isCC;       // ����

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

            // ��Ʈ���� �ʿ��� �޸� ������(Byte)
            MemoryStream ms = new MemoryStream(sizeof(int) + sizeof(int));

            // �� �������� Byte �������� ��ȯ, �������� ���� ������
            ms.Write(BitConverter.GetBytes(ct._attackDmg), 0, sizeof(int));
            ms.Write(BitConverter.GetBytes(ct._coefficient), 0, sizeof(int));
            ms.Write(BitConverter.GetBytes(ct._animNum), 0, sizeof(int));
            ms.Write(BitConverter.GetBytes(ct._cooltime), 0, sizeof(int));

            foreach (bool tmp in ct._isCC)
            {
                ms.Write(BitConverter.GetBytes(tmp), 0, sizeof(bool));
            }

            // ������� ��Ʈ���� �迭 �������� ��ȯ
            return ms.ToArray();
        }

        // ������ȭ
        public static object Deserialize(byte[] bytes)
        {
            stSkill ct = new stSkill();

            // ����Ʈ �迭�� �ʿ��� ��ŭ �ڸ���, ���ϴ� �ڷ������� ��ȯ
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
