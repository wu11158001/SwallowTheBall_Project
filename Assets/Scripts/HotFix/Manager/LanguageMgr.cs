using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageMgr : UnitySingleton<LanguageMgr>
{
    /*
     * 0 = 英文
     * 1 = 繁體中文
     */
    private const string SWALLOW_LANGUAGE = "Swallow_Language";         // 本地紀錄

    public int CurrLanguage { get; private set; }                       // 當前語言

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        int localLanguage = PlayerPrefs.GetInt(SWALLOW_LANGUAGE);
        ChangeLanguage(localLanguage);
    }

    /// <summary>
    /// 更換語言
    /// </summary>
    /// <param name="index"></param>
    public void ChangeLanguage(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        PlayerPrefs.SetInt(SWALLOW_LANGUAGE, index);
        CurrLanguage = index;
    }
}
