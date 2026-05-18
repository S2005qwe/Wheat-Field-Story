using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
/// <summary>
/// 实现玩家进入区域改变物体透明度
/// </summary> 

//限制必须要有SpriteRenderer组件
[RequireComponent(typeof(SpriteRenderer))]


public class ItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 逐渐恢复颜色
    /// </summary>
    public void FadeIn()
    {
        //目标颜色
        Color targetColor = new Color(1, 1, 1, 1);

        //使用DoTween插件
        spriteRenderer.DOColor(targetColor, Settings.itemFadeDuration);
    }

    /// <summary>
    /// 逐渐半透明
    /// </summary>
    public void FadeOut()
    {
        //目标颜色
        Color targetColor = new Color(1, 1, 1, Settings.targetAlpha);

        //使用DoTween插件
        spriteRenderer.DOColor(targetColor, Settings.itemFadeDuration);
    }

}
