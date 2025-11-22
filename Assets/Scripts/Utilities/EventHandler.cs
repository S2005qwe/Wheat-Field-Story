using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler 
{
    //更新UI
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location, list);
    }


    //转换屏幕坐标，用于把背包物品放在地上
    public static event Action<int, Vector3> InstaniateItemInScene;
    public static void CallInstantiateItemInScene(int ID , Vector3 pos)
    {
        InstaniateItemInScene?.Invoke(ID, pos);
    }

    //举起物品
    public static event Action<ItemDetails,bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails,bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails,isSelected);
    }

    public static event Action<int, int> GameMinuteEvent;


    public static void CallGameMinuteEvent(int minute,int hour)
    {
        GameMinuteEvent?.Invoke(minute,hour);
    }

    public static event Action<int, int,int,int ,Season>GameDataEvent;
    public static void CallGameDataEvent(int hour,int day,int month,int year,Season season)
    {
        GameDataEvent?.Invoke(hour,day,month,year,season);  
    }

}
