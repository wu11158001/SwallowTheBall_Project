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
    LobbyView,
}

public class ViewMgr : UnitySingleton<ViewMgr>
{
    private Camera _mainCamera;
    private Canvas _canvas;
    private RectTransform _canvasRt;

    private Dictionary<ViewEnum, GameObject> _cachedView = new();

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
    /// 初始化View
    /// </summary>
    /// <param name="rt"></param>
    public void InitView(RectTransform rt)
    {
        rt.offsetMax = Vector2.zero;
        rt.offsetMin = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        rt.eulerAngles = Vector3.zero;
        rt.localScale = Vector3.one;
        rt.name = rt.name.Replace("(Clone)", "");
        rt.SetSiblingIndex(_canvasRt.childCount + 1);
    }

    /// <summary>
    /// 開啟介面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="viewEnum"></param>
    /// <param name="callBack"></param>
    public void OpenView<T>(ViewEnum viewEnum, UnityAction<T> callBack = null) where T : Component
    {
        /*
         * 創建場景處理
         * */
        void CreateViewHandle(RectTransform createView)
        {
            InitView(createView);

            if (callBack != null)
            {
                T component = createView.GetComponent<T>();
                if (component != null)
                {
                    callBack?.Invoke(component);
                }
                else
                {
                    Debug.LogError($"{viewEnum}: 介面不存在 Component");
                }
            }
        }
        Debug.Log("創建介面");
        if (_cachedView.ContainsKey(viewEnum))
        {
            RectTransform rt = Instantiate(_cachedView[viewEnum], _canvasRt).GetComponent<RectTransform>();
            CreateViewHandle(rt);
        }
        else
        {
            Addressables.InstantiateAsync($"Prefab/View/{viewEnum}.prefab", _canvasRt).Completed += (handle) =>
            {
                if (handle.Result != null)
                {
                    RectTransform rt = handle.Result.GetComponent<RectTransform>();
                    CreateViewHandle(rt);

                    GameObject obj = handle.Result;
                    _cachedView.Add(viewEnum, obj);

                    Addressables.Release(handle);
                }
                else
                {
                    Debug.LogError("$無法加載介面:{viewEnum}");
                }
            };
        }
    }
}
