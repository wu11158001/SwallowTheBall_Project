using UnityEngine;

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
        SceneMgr.I.ChangeScene(SceneEnum.Lobby);
    }
}
