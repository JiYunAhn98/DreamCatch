using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraWork : MonoBehaviour
{
    #region [ Private Fields ]
    [SerializeField] private float distance = 7.0f; // ���� xz����� �Ÿ�
    [SerializeField] private float height = 3.0f;   // ī�޶� ��󺸴� �󸶳� ���� ������ �� ����
    [SerializeField] private Vector3 centerOffset = Vector3.zero;   // ī�޶� ����� �Ĵٺ��� Rotation Offset ����
    [SerializeField] private float smoothSpeed = 0.125f;    // ���������ӵ�

    int _followTargetNum;
    bool isFollowing;                       // Ÿ���� ��ġ�ų� ī�޶� �ٲ�� �ٽ� �����ϱ� ���� �÷���, ���� ���� �� �ִ� ��Ȳ���� �ľ�
    Transform cameraTransform;              // ī�޶� Ÿ���� transform ���� (cache��� ǥ���ϴ���.)
    Vector3 cameraOffset = Vector3.zero;    // ī�޶� Offset ���� (cache��� ǥ���ϴ���.)
    GameObject[] _players;
    #endregion [ Private Fields ]

    #region [ MonoBehaviour Callbacks ]
    void LateUpdate()
    {
        // ���� �� �ִ� ��Ȳ�̶�� ���󰣴�.
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
    /// ���󰡱� ������ �� 1ȸ ����
    /// �Ϲ������� Photon Network���� �����ϴ� �ν��Ͻ��� ���� ������ ����� ���� ������ �� �� �� ���� ��� ����մϴ�.
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
    /// �������
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
    /// Camera�� �ش� ��ü�� ���������ϸ� ���󰣴�. ���ӽ����ʿ�, �������� ī�޶� ������
    /// </summary>
    void Follow()
    {
        transform.position = Vector3.Lerp(cameraTransform.position + Vector3.up * height + Vector3.back * distance, this.transform.position + this.transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);

        transform.LookAt(cameraTransform.position + centerOffset);
    }

    /// <summary>
    /// Camera�� �ش� ��ü�� �ٷ� ���󰣴�. ī�޶�� ����� �Ÿ� ���� �� 1ȸ �۵�, ���� �ε巯�� �������� ������ ������ ���ӽ���
    /// </summary>
    void Cut()
    {
        transform.position = cameraTransform.position + Vector3.up * height + Vector3.back * distance + this.transform.TransformVector(cameraOffset);

        transform.LookAt(cameraTransform.position + centerOffset);

    }
    #endregion
}
