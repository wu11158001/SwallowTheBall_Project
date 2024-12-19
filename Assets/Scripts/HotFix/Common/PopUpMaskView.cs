using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public abstract class PopUpMaskView : MonoBehaviour
{
    [SerializeField]
    Button Mask_Btn;
    [SerializeField]
    RectTransform SlideView;

    private const float AlphaMax = 200;                 // 遮罩透明值最大值
    private const float EffectTime = 1.0f;              // 效果完成時間

    private void Start()
    {
        StartCoroutine(IMaskFadeIn());
    }

    /// <summary>
    /// 遮罩淡出
    /// </summary>
    public void OnMaskFadeOut()
    {
        StartCoroutine(IMaskFadeOut());
    }

    /// <summary>
    /// 遮罩淡入
    /// </summary>
    /// <returns></returns>
    private IEnumerator IMaskFadeIn()
    {
        Color color = Mask_Btn.image.color;
        DateTime startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalSeconds <= EffectTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / EffectTime;
            color.a = Mathf.Lerp(0, AlphaMax, progress);
            Mask_Btn.image.color = color;

            yield return null;
        }

        color.a = AlphaMax;
        Mask_Btn.image.color = color;

        // 遮罩按鈕
        Mask_Btn.onClick.AddListener(() =>
        {
            Mask_Btn.onClick.RemoveAllListeners();
            OnMaskFadeOut();
        });
    }

    /// <summary>
    /// 遮罩淡出
    /// </summary>
    /// <returns></returns>
    private IEnumerator IMaskFadeOut()
    {
        Color color = Mask_Btn.image.color;
        DateTime startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalSeconds <= EffectTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / EffectTime;
            color.a = Mathf.Lerp(AlphaMax, 0, progress);
            Mask_Btn.image.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}
