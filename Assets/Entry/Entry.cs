using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Entry : MonoBehaviour
{
    private static Entry _instance;
    public static Entry I { get { return _instance; } }

    [Header("使用Debug工具")]
    public bool IsUsingDebug;

    // 當前需要更新的資源大小
    private long updateSize = 0;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        if (IsUsingDebug)
        {
            //Debug工具初始化
            Reporter.I.Initialize();
            Reporter.I.show = false;
        }
    }

    private IEnumerator Start()
    {
        yield return CheckForUpdates();

        Assembly ass = null;

#if UNITY_EDITOR
        ass = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotFix");
        LoadLauncherMgr(ass);
#else
        Addressables.LoadAssetAsync<TextAsset>("DLL/HotFix.dll.bytes").Completed += ((handle) =>
        {
            ass = Assembly.Load(handle.Result.bytes);
            LoadLauncherMgr(ass);
        });
#endif
    }

    /// <summary>
    /// 載入啟動腳本
    /// </summary>
    /// <param name="ass"></param>
    private void LoadLauncherMgr(Assembly ass)
    {
        Type type = ass.GetType("LauncherMgr");
        GameObject langcherMgrObj = new GameObject("LauncherMgr");
        langcherMgrObj.AddComponent(type);
        type.GetMethod("GameLauncher").Invoke(langcherMgrObj.GetComponent(type), null);
    }

    /// <summary>
    /// 檢查是否有可更新資源
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckForUpdates()
    {
        // 檢查是否有 Catalog 更新
        AsyncOperationHandle<List<string>> checkHandle = Addressables.CheckForCatalogUpdates();
        yield return checkHandle;

        if (checkHandle.Status == AsyncOperationStatus.Succeeded && checkHandle.Result.Count > 0)
        {
            Debug.Log("發現更新的目錄，開始檢查下載大小...");

            // 獲取更新資源的大小
            AsyncOperationHandle<long> getSizeHandle = Addressables.GetDownloadSizeAsync(checkHandle.Result);
            yield return getSizeHandle;

            if (getSizeHandle.Status == AsyncOperationStatus.Succeeded)
            {
                updateSize = getSizeHandle.Result;

                if (updateSize > 0)
                {
                    Debug.Log($"有 {updateSize / (1024 * 1024)} MB 的資源需要更新。");

                    // 啟動資源更新過程
                    yield return UpdateResources(checkHandle.Result);
                }
                else
                {
                    Debug.Log("沒有資源需要更新。");
                }
            }
        }
        else
        {
            Debug.Log("沒有發現需要更新的目錄。");
        }

        Addressables.Release(checkHandle);
    }

    /// <summary>
    /// 更新資源
    /// </summary>
    /// <param name="catalogs"></param>
    /// <returns></returns>
    private IEnumerator UpdateResources(List<string> catalogs)
    {
        // 更新目錄
        AsyncOperationHandle<List<IResourceLocator>> updateHandle = Addressables.UpdateCatalogs(catalogs);
        yield return updateHandle;

        if (updateHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("目錄更新成功，開始下載資源...");

            // 下載依賴資源
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(catalogs, Addressables.MergeMode.UseFirst);
            while (!downloadHandle.IsDone)
            {
                Debug.Log($"下載進度：{downloadHandle.PercentComplete * 100}%");
                yield return null;
            }

            if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("資源下載完成！");
            }
            else
            {
                Debug.LogError("資源下載失敗！");
            }

            Addressables.Release(downloadHandle);
        }
        else
        {
            Debug.LogError("目錄更新失敗！");
        }

        Addressables.Release(updateHandle);
    }
}
