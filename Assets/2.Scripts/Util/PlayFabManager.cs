using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    // ����� �޾ƿ��� �ֽ�ȭ�ϴµ� ����ϰ� �߿��� �����͵��� Ŭ���忡�� �ϵ��� ����.
    public void SetValue()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>()
                {
                    new StatisticUpdate { StatisticName = "1", Value = 1 },
                }
            },
            (result) => { },
            (error) => { }
        );
    }
}
