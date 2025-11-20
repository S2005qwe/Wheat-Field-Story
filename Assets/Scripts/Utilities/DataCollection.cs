using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemId; //id

    public string itemName;//名字

    public ItemType itemType;//类型

    public Sprite itemIcon; //图标

    public Sprite itemOnWorldSprite; //边框

    public string itemDescription;  //描述

    public int itemUseRadius; //使用范围

    public bool canPickedUp; //拾取

    public bool canDropped;  //扔下

    public bool canCarried; //举起

    public int itemPrice;
    [Range(0,1)]
    public float sellPercentage;
}

//背包物品
//为什么不用class,使用class需要判断是否为空
//struct默认不可为空
[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}

[System.Serializable]
public class AnimatorType
{
    public PartType partType;
    public PartName partName;
    public AnimatorOverrideController overrideController;
}
