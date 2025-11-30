public enum ItemType
{ 
    Seed,Commodity,Furniture,

   //锄头   砍树工具  砸石头工具 割草工具  浇水工具  收割工具
    HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectTool,

    //杂草
    ReapableScenery

}

public enum SlotType
{
    Bag,Box,Shop


}
public enum InventoryLocation
{
    Player,Box
}

public enum PartType
{
    None,Carry,Hoe,Break,
}
public enum PartName
{
    Body,Hair,Arm,Tool
}

public enum Season
{
    春天,夏天,秋天,冬天
}

public enum GridType
{
    Diggable,DropItem,PlaceFurniture,NPCObstacle
}