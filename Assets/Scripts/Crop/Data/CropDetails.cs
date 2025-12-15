using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CropDetails 
{
    public int seedItemID;

    [Header("不同阶段需要的天数")]
    public int[] growthDays;
    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }
    [Header("不同生长阶段物品的Prefab")]
    public GameObject[] growPrefabs;

    [Header("不同阶段的图片")]
    public Sprite[] growSprites;

    [Header("可种植的季节")]
    public Season[] seasons;

    [Space]
    [Header("收割工具")]
    public int[] harvestToolItemID;
    [Header("每种工具使用次数")]
    public int[] requireActionCount;
    [Header("转换新物品ID")]
    public int transferItemID;

    [Space]
    [Header("收割果实信息")]
    public int[] producedItemID;              //生产个数

    public int[] producedMinAmount;           //生产个数最小值

    public int[] producedMaxAmount;           //生产个数最大值

    public Vector2 spawnRadius;               //生长范围

    [Header("再次生长时间")]
    public int daysToRegrow;

    public int regrowTimes;                   //可再次生长次数

    [Header("Options")]
    public bool generateAtPlayerPosition;     //是否在Player身上生成

    public bool hasAnimation;  //是否有动画

    public bool hasParticalEffect;            //是否有粒子特效

    //TODO：特效，声音等
    //public ParticaleEffectType effectType;  //特效类型

    public Vector3 effectPos;                 //特效坐标

    //public SoundName soundEffect;
}
