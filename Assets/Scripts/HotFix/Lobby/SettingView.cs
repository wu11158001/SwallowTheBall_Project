using UnityEngine;
using UnityEngine.UI;


public class SettingView : BasePopUpView
{
    [Space(30)]
    [Header("語言")]
    [SerializeField]
    Toggle en_Tog;
    [SerializeField]
    Toggle zhTw_Tog;

    private void Start()
    {
        EventListener();

        switch (LanguageMgr.I.CurrLanguage)
        {
            // 英文
            case 0:
                en_Tog.isOn = true;
                break;

            // 繁體中文
            case 1:
                zhTw_Tog.isOn = true;
                break;

            // 預設(英文)
            default:
                en_Tog.isOn = true;
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void EventListener()
    {
        #region 語言

        // 英文
        en_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                LanguageMgr.I.ChangeLanguage(0);
            }
        });

        // 繁體中文
        zhTw_Tog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                LanguageMgr.I.ChangeLanguage(1);
            }
        });

        #endregion
    }
}
