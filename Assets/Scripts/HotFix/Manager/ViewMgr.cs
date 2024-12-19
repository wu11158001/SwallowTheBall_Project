using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 介面Enum
/// </summary>
public enum ViewEnum
{
    LobbyView,                      // 大廳
    SettingView                     // 設定
}

public class ViewMgr : UnitySingleton<ViewMgr>
{
    private Camera _mainCamera;
    private Canvas _canvas;
    private RectTransform _canvasRt;

    private Dictionary<ViewEnum, RectTransform> _cachedView = new();

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        _mainCamera = Camera.main;
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _canvasRt = _canvas.GetComponent<RectTransform>();

        _cachedView.Clear();
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
        if (_cachedView.ContainsKey(viewEnum))
        {
            RectTransform rt = _cachedView[viewEnum];
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

                    _cachedView.Add(viewEnum, rt);
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
