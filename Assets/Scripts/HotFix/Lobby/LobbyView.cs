using UnityEngine;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour
{
    [SerializeField]
    Button Setting_Btn;

    void Start()
    {
        EventListener();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void EventListener()
    {
        // 設定按鈕
        Setting_Btn.onClick.AddListener(() =>
        {
            ViewMgr.I.OpenView<RectTransform>(ViewEnum.SettingView);
        });
    }
}
