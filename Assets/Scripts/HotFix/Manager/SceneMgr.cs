using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

/// <summary>
/// 場景Enum
/// </summary>
public enum SceneEnum
{
    Entry,
    Lobby,
}

public class SceneMgr : UnitySingleton<SceneMgr>
{
    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 轉換場景
    /// </summary>
    /// <param name="sceneEnum"></param>
    public void ChangeScene(SceneEnum sceneEnum)
    {
        Addressables.LoadSceneAsync($"Assets/Scenes/{sceneEnum}.unity", LoadSceneMode.Single).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"進入場景:{sceneEnum} !");

                ViewMgr.I.Init();

                // 產生場景初始介面
                switch (sceneEnum)
                {
                    // 大廳
                    case SceneEnum.Lobby:
                        ViewMgr.I.OpenView<RectTransform>(ViewEnum.LobbyView);
                        break;
                }
            }
            else
            {
                Debug.LogError($"{sceneEnum} 場景載入失敗 ！");
            }
        };
    }
}
