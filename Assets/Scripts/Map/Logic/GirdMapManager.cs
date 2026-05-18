using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using SFarm.CropPlant;
using SFarm.Save;

namespace SFarm.Map
{
    public class GirdMapManager : Singleton<GirdMapManager>,ISaveable
    {
        [Header("种地瓦片切换信息")]
        public RuleTile digTile;
        public RuleTile waterTile;
        private Tilemap digTilemap;
        private Tilemap waterTilemap;

        [Header("地图信息")]
        public List<MapData_SO> mapDatalist;

        private Season currentSeason;

        //场景名字+坐标和对应的瓦片信息
        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

        //场景是否第一次加载
        private Dictionary<string, bool> firstLoadDict = new Dictionary<string, bool>();

        //杂草列表
        private List<ReapItem> itemInRadius;

        private Grid currentGrid;

        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrentMap += RefreshMap;
        }
        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= RefreshMap;
        }


        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();

            foreach (var mapData in mapDatalist)
            {
                firstLoadDict.Add(mapData.sceneName, true);
                InitTileDetailsDict(mapData);
            }
        }

        private void OnAfterSceneLoadEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            digTilemap = GameObject.FindGameObjectWithTag("Dig").GetComponent<Tilemap>();
            waterTilemap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();

            if (firstLoadDict[SceneManager.GetActiveScene().name])
            {
                //预先生成农作物
                EventHandler.CallGenerateCropEvent();
                firstLoadDict[SceneManager.GetActiveScene().name] = false;
            }
            
            RefreshMap();
        }

        /// <summary>
        /// 每天执行一次
        /// </summary>
        /// <param name="day"></param>
        /// <param name="season"></param>
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;

            foreach (var tile in tileDetailsDict)
            {
                if (tile.Value.daysSinceWatered > -1)
                {
                    tile.Value.daysSinceWatered = -1;
                }
                if (tile.Value.daysSinceDug > -1)
                {
                    tile.Value.daysSinceDug++;
                }
                //超期消除挖坑
                if (tile.Value.daysSinceDug > 5 && tile.Value.seedItemID == -1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                }
                if (tile.Value.seedItemID != -1)
                {
                    tile.Value.growthDays++;
                }
            }

            RefreshMap();
        }

        /// <summary>
        /// 根据地图信息生成字典
        /// </summary>
        /// <param name="mapData"></param>
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    gridX = tileProperty.tileCoordinate.x,
                    gridY = tileProperty.tileCoordinate.y
                };

                //字典的Key
                string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + mapData.sceneName;

                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (tileProperty.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                }

                if (GetTileDetails(key) != null)
                {
                    tileDetailsDict[key] = tileDetails;
                }
                else
                {
                    tileDetailsDict.Add(key, tileDetails);
                }
            }
        }


        /// <summary>
        /// 根据key返回瓦片信息
        /// </summary>
        /// <param name="key">x+y+地图名字</param>
        /// <returns></returns>
        public TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.ContainsKey(key))
            {
                return tileDetailsDict[key];
            }
            else return null;
        }

        /// <summary>
        /// 根据鼠标网格坐标返回瓦片信息
        /// </summary>
        /// <param name="mouseGridPos">鼠标网格坐标</param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y + "y" + SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }


        /// <summary>
        /// 执行实际工具或物品功能
        /// </summary>
        /// <param name="mouseWorldPos">鼠标坐标</param>
        /// <param name="itemDetails">物品信息</param>
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);

            if (currentTile != null)
            {
                Crop currentCrop = GetCropObject(mouseWorldPos);

                //TODO:物品使用实际功能
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:             //种子
                        EventHandler.CallPlantSeedEvent(itemDetails.itemId, currentTile);
                        EventHandler.CallDropItemEvent(itemDetails.itemId, mouseWorldPos, itemDetails.itemType);
                        EventHandler.CallPlaySoundEvent(SoundName.Plant);
                        break;
                    case ItemType.Commodity:        //商品
                        EventHandler.CallDropItemEvent(itemDetails.itemId, mouseWorldPos, itemDetails.itemType);
                        break;
                    case ItemType.HoeTool:          //锄头
                        SetDigGround(currentTile);
                        currentTile.daysSinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        //加音效
                        EventHandler.CallPlaySoundEvent(SoundName.Hoe);
                        break;
                    case ItemType.WaterTool:    //水壶
                        SetWaterGround(currentTile);
                        currentTile.daysSinceWatered = 0;
                        //音效
                        EventHandler.CallPlaySoundEvent(SoundName.Water);
                        break;
                    case ItemType.BreakTool:    //十字镐
                    case ItemType.ChopTool:     //斧头
                        //执行收割方法
                        currentCrop.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                        break;
                    case ItemType.CollectTool:  //菜篮子
                        //Crop currentCrop = GetCropObject(mouseWorldPos);
                        //执行收割方法
                        currentCrop.ProcessToolAction(itemDetails, currentTile);
                        break;
                    case ItemType.ReapTool:     //镰刀
                        var reapCount = 0;
                        for (int i = 0; i < itemInRadius.Count; i++)
                        {
                            EventHandler.CallParticalEffectEvent(ParticaleEffectType.ReapableScenery, itemInRadius[i].transform.position + Vector3.up);
                            itemInRadius[i].SpawnHarvestItems();
                            Destroy(itemInRadius[i].gameObject);
                            reapCount++;
                            if (reapCount >= Settings.reapAmount)
                                break;
                        }
                        EventHandler.CallPlaySoundEvent(SoundName.Reap);
                        break;
                    case ItemType.Furniture:
                        //在地图上生成物品 ItemManager
                        //移除当前物品（图纸） InventoryManager
                        //移除资源物品 InventoryManager
                        EventHandler.CallBuildFurnitureEvent(itemDetails.itemId, mouseWorldPos);
                        break;
                }

                UpdateTileDetails(currentTile);
            }
        }

        /// <summary>
        /// 通过物理方法判断鼠标点击位置的农作物
        /// </summary>
        /// <param name="mouseWorldPos">鼠标坐标</param>
        /// <returns></returns>
        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);

            Crop currentCrop = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                    currentCrop = colliders[i].GetComponent<Crop>();
            }
            return currentCrop;
        }

        /// <summary>
        /// 返回工具范围内的杂草
        /// </summary>
        /// <param name="tool">物品信息</param>
        /// <returns></returns>
        public bool HaveReapableItemsInRadius(Vector3 mouseWorldPos, ItemDetails tool)
        {
            itemInRadius = new List<ReapItem>();

            Collider2D[] colliders = new Collider2D[20];

            Physics2D.OverlapCircleNonAlloc(mouseWorldPos, tool.itemUseRadius, colliders);

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<ReapItem>())
                        {
                            var item = colliders[i].GetComponent<ReapItem>();
                            itemInRadius.Add(item);
                        }
                    }
                }
            }
            return itemInRadius.Count > 0;
        }

        /// <summary>
        /// 显示挖坑瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetDigGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (digTilemap != null)
                digTilemap.SetTile(pos, digTile);
        }
        /// <summary>
        /// 显示浇水瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (waterTilemap != null)
                waterTilemap.SetTile(pos, waterTile);
        }

        /// <summary>
        /// 更新瓦片信息
        /// </summary>
        /// <param name="tileDetails">tileDetails</param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
            }
        }


        /// <summary>
        /// 刷新当前地图
        /// </summary>
        private void RefreshMap()
        {
            if (digTilemap != null)
                digTilemap.ClearAllTiles();
            if (waterTilemap != null)
                waterTilemap.ClearAllTiles();

            foreach (var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }

            DisplayMap(SceneManager.GetActiveScene().name);
        }


        /// <summary>
        /// 显示地图瓦片
        /// </summary>
        /// <param name="sceneName"></param>
        private void DisplayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;

                if (key.Contains(sceneName))
                {
                    if (tileDetails.daysSinceDug > -1)
                        SetDigGround(tileDetails);
                    if (tileDetails.daysSinceWatered > -1)
                        SetWaterGround(tileDetails);
                    if (tileDetails.seedItemID > -1)
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemID, tileDetails);
                }
            }
        }



        /// <summary>
        /// 根据场景名字构建网络范围，输出范围和原点
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <param name="gridDimensions">网络范围</param>
        /// <param name="gridOrigin">网格原点</param>
        /// <returns>是否有当前场景的信息</returns>
        public bool GetGridDimensions(string sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)
        {
            gridDimensions = Vector2Int.zero;
            gridOrigin = Vector2Int.zero;

            foreach (var mapData in mapDatalist)
            {
                if (mapData.sceneName == sceneName)
                {
                    gridDimensions.x = mapData.gridWidth;
                    gridDimensions.y = mapData.gridHeight;

                    gridOrigin.x = mapData.originX;
                    gridOrigin.y = mapData.originY;

                    return true;
                }
            }
            return false;
        }

        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.tileDetailsDict = this.tileDetailsDict;
            saveData.firstLoadDict = this.firstLoadDict;
            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.tileDetailsDict = saveData.tileDetailsDict;
            this.firstLoadDict = saveData.firstLoadDict;
        }
    }
}

