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
    None, Carry, Hoe, Break, Water, Chop, Collect, Reap
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
public enum ParticaleEffectType
{
    None,LeavesFalling01,LeavesFalling02,Rock,ReapableScenery
}
public enum GameState
{
    GamePlay,Pause
}

public enum LightShift
{
    Morning,Night
}
public enum SoundName
{
    none,FootStepSoft,FootStepHard,
    Axe,Pickaxe,Hoe,Reap,Water,Basket,Chop,
    Pickup,Plant,TreeFalling,Rustle,
    AmbientCountryside1, AmbientCountryside2,MusicCalm1, MusicCalm2, MusicCalm3, MusicCalm4, MusicCalm5, MusicCalm6,AmbientIndoor1
}