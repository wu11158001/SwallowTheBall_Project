using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AddressableAssets;
using System;

/// <summary>
/// 一般介面Enum
/// </summary>
public enum ViewEnum
{
    LobbyView,                      // 大廳
    SettingView                     // 設定
}

/// <summary>
/// 常駐介面Enum
/// </summary>
public enum PermanentViewEnum
{
    SceneLoadView,                  // 場景轉換介面
}

public class ViewMgr : UnitySingleton<ViewMgr>
{
    private Camera _mainCamera;
    private Canvas _canvas;
    private RectTransform _canvasRt;

    private Dictionary<ViewEnum, RectTransform> _normalView = new();                    // 一般介面
    private Dictionary<PermanentViewEnum, RectTransform> _permanentView = new();        // 功能介面

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 介面腳本準備
    /// </summary>
    /// <returns></returns>
    public IEnumerator IPrepare()
    {
        _permanentView.Clear();

        foreach (var permanentEnum in Enum.GetValues(typeof(PermanentViewEnum)))
        {
            var handle = Addressables.LoadAssetAsync<GameObject>($"Prefab/View/{permanentEnum}.prefab");

            yield return handle;

            RectTransform rt = handle.Result.GetComponent<RectTransform>();

            _permanentView.Add((PermanentViewEnum)permanentEnum, rt);
            Addressables.Release(handle);
        }

        Debug.Log("介面腳本準備完成。");
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        _mainCamera = Camera.main;
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _canvasRt = _canvas.GetComponent<RectTransform>();

        _normalView.Clear();
    }

    /// <summary>
    /// 獲取當前主攝影機
    /// </summary>
    /// <returns></returns>
    public Camera GetMainCamera()
    {
        return _mainCamera;
    }

    /// <summary>
    /// 獲取當前Canvas
    /// </summary>
    /// <returns></returns>
    public Canvas GetCanvas()
    {
        return _canvas;
    }

    /// <summary>
    /// 獲取當前Canvas RectTransform
    /// </summary>
    /// <returns></returns>
    public RectTransform GetCanvasRectTransform()
    {
        return _canvasRt;
    }

    /// <summary>
    /// 開啟場景轉換介面
    /// </summary>
    public RectTransform OpenSceneLoadView()
    {
        RectTransform sceneLoadView = _permanentView[PermanentViewEnum.SceneLoadView];
        RectTransform rt = Instantiate(sceneLoadView, _canvasRt).GetComponent<RectTransform>();
        CreateViewHandle<RectTransform>(rt);

        return rt;
    }

    /// <summary>
    /// 產生介面處理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rt"></param>
    /// <param name="callBack"></param>
    public void CreateViewHandle<T>(RectTransform rt, UnityAction<T> callBack = null) where T : Component
    {
        rt.gameObject.SetActive(true);
        rt.offsetMax = Vector2.zero;
        rt.offsetMin = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        rt.eulerAngles = Vector3.zero;
        rt.localScale = Vector3.one;
        rt.name = rt.name.Replace("(Clone)", "");
        rt.SetSiblingIndex(_canvasRt.childCount + 1);

        if (callBack != null)
        {
            T component = rt.GetComponent<T>();
            if (component != null)
            {
                callBack?.Invoke(component);
            }
            else
            {
                Debug.LogError($"{rt.name}: 介面不存在 Component");
            }
        }
    }

    /// <summary>
    /// 開啟介面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="viewEnum"></param>
    /// <param name="callBack"></param>
    public void OpenView<T>(ViewEnum viewEnum, UnityAction<T> callBack = null) where T : Component
    {
        if (_normalView.ContainsKey(viewEnum))
        {
            RectTransform rt = _normalView[viewEnum];
            CreateViewHandle(rt, callBack);
        }
        else
        {
            Addressables.LoadAssetAsync<GameObject>($"Prefab/View/{viewEnum}.prefab").Completed += (handle) =>
            {
                if (handle.Result != null)
                {
                    RectTransform rt = Instantiate(handle.Result, _canvasRt).GetComponent<RectTransform>();
                    CreateViewHandle(rt, callBack);

                    _normalView.Add(viewEnum, rt);
                    Addressables.Release(handle);
                }
                else
                {
                    Debug.LogError($"無法加載介面:{viewEnum}");
                }
            };
        }
    }
}
