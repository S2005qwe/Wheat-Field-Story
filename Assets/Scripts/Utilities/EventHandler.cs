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
}
