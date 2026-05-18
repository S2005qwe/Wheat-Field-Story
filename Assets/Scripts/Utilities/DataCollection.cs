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

[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;
    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    //瓦片地图
    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}


//存储场景物品数据
[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;
}

[System.Serializable]
public class SceneFurniture
{
    public int itemID;
    public SerializableVector3 position;

    public int boxIndex;
}
[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;
    public GridType gridType;
    public bool boolTypeValue;
}
[System.Serializable]
public class TileDetails
{
    public int gridX, gridY;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNPCObstacle;

    public int daysSinceDug = -1;
    public int daysSinceWatered = -1;
    public int seedItemID = -1;
    public int growthDays = -1;
    public int daysSinceLastHarvest = -1;

}
[System.Serializable]
public class NPCPosition
{
    public Transform npc;

    public string startScene;

    public Vector3 position;
}

[System.Serializable]
public class SceneRoute
{
    public string fromSceneName;

    public string gotoSceneName;

    public List<ScenePath> scenePathList;
}

[System.Serializable]
public class ScenePath
{
    public string sceneName;

    public Vector2Int fromGridCell;

    public Vector2Int gotoGridCell;
}
