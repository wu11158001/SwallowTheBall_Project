using UnityEngine;
using System.Collections;

public class LauncherMgr : UnitySingleton<LauncherMgr>
{
    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 遊戲啟動
    /// </summary>
    public void GameLauncher()
    {
        Debug.Log("遊戲啟動 !!!");

        StartCoroutine(IProjectInit());
    }

    /// <summary>
    /// 專案初始準備
    /// </summary>
    /// <returns></returns>
    private IEnumerator IProjectInit()
    {
        yield return ViewMgr.I.IPrepare();

        LanguageMgr.I.Init();
        SceneMgr.I.ChangeScene(SceneEnum.Lobby);
    }
}
