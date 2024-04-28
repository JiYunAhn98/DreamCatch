using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using UnityEngine.SceneManagement;
public class SceneControllManager : MonoSingleton<SceneControllManager>
{
    eSceneName _curScene;

    public void LoadScene(eSceneName nextScene)
    {
        _curScene = nextScene;
        SceneManager.LoadSceneAsync(nextScene.ToString());
    }
}
