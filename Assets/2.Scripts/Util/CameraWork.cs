using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraWork : MonoBehaviour
{
    #region [ Private Fields ]
    [SerializeField] private float distance = 7.0f; // 대상과 xz평면의 거리
    [SerializeField] private float height = 3.0f;   // 카메라가 대상보다 얼마나 위에 존재할 지 정도
    [SerializeField] private Vector3 centerOffset = Vector3.zero;   // 카메라가 대상을 쳐다보는 Rotation Offset 정도
    [SerializeField] private float smoothSpeed = 0.125f;    // 선형보간속도

    int _followTargetNum;
    bool isFollowing;                       // 타겟을 놓치거나 카메라가 바뀌면 다시 연결하기 위한 플래그, 현재 따라갈 수 있는 상황인지 파악
    Transform cameraTransform;              // 카메라 타겟의 transform 저장 (cache라고 표현하더라.)
    Vector3 cameraOffset = Vector3.zero;    // 카메라 Offset 저장 (cache라고 표현하더라.)
    GameObject[] _players;
    #endregion [ Private Fields ]

    #region [ MonoBehaviour Callbacks ]
    void LateUpdate()
    {
        // 따라갈 수 있는 상황이라면 따라간다.
        if (cameraTransform != null && !cameraTransform.gameObject.activeSelf)
        {
            ChangeCameraTarget();
        }
        if (cameraTransform != null && isFollowing)
        {
            Follow();
        }
    }
    #endregion [ MonoBehaviour Callbacks ]

    #region [ Public Methods ]

    /// <summary>
    /// 따라가기 시작할 때 1회 실행
    /// 일반적으로 Photon Network에서 관리하는 인스턴스와 같이 무엇을 따라야 할지 선택할 때 알 수 없는 경우 사용합니다.
    /// </summary>
    public void OnStartFollowing(Transform target)
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
        cameraTransform = target;
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].transform == target)
            {
                _followTargetNum = i;
                break;
            }
        }
        isFollowing = true;
        Cut();
    }
    /// <summary>
    /// 관전모드
    /// </summary>
    public void ChangeCameraTarget()
    {
        if (_players.Length != PhotonNetwork.PlayerList.Length)
        {
            _players = GameObject.FindGameObjectsWithTag("Player");
        }
        isFollowing = true;
        while (!_players[_followTargetNum].activeSelf)
        {
            _followTargetNum = (_followTargetNum + 1) % _players.Length;
        }
        cameraTransform = _players[_followTargetNum].transform;
        Follow();
    }
    #endregion [ Public Methods ]

    #region Private Methods

    /// <summary>
    /// Camera가 해당 물체를 선형보간하며 따라간다. 지속실행필요, 스무스한 카메라 움직임
    /// </summary>
    void Follow()
    {
        transform.position = Vector3.Lerp(cameraTransform.position + Vector3.up * height + Vector3.back * distance, this.transform.position + this.transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);

        transform.LookAt(cameraTransform.position + centerOffset);
    }

    /// <summary>
    /// Camera가 해당 물체를 바로 따라간다. 카메라와 대상의 거리 고정 시 1회 작동, 만약 부드러운 움직임을 원하지 않으면 지속실행
    /// </summary>
    void Cut()
    {
        transform.position = cameraTransform.position + Vector3.up * height + Vector3.back * distance + this.transform.TransformVector(cameraOffset);

        transform.LookAt(cameraTransform.position + centerOffset);

    }
    #endregion
}
