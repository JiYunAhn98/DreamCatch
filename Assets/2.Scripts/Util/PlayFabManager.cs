using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    // 목록을 받아오고 최신화하는데 사용하고 중요한 데이터들은 클라우드에서 하도록 하자.
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
