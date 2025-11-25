using SFarm.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrfabs;  //物品预制体

        private Transform itemParent; //用于物品放在父物体下

        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();

        private void OnEnable()
        {
            EventHandler.InstaniateItemInScene += OnInstaniateItemInScene;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;

        }
        private void OnDisable()
        {
            EventHandler.InstaniateItemInScene -= OnInstaniateItemInScene;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        }

        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
        }


        //加载场景后寻找父物体
        private void OnAfterSceneLoadedEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
        }


        /// <summary>
        /// 在指定坐标位置生成物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="pos">世界坐标</param>
        private void OnInstaniateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPrfabs, pos, Quaternion.identity, itemParent);
            item.itemID = ID;
        }


        /// <summary>
        /// 获取场景中全部物品
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();

            //拿到当前场景物品
            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem()
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };
                currentSceneItems.Add(sceneItem);
            }
            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //找到数据就更新item数据列表
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;

            }
            else
            {
                //如果是新场景
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }

        /// <summary>
        /// 刷新重建当前场景物品
        /// </summary>
        private void RecreateAllItems()
        {
            //创建一个列表存储当前物品
            List<SceneItem> currentSceneItems = new List<SceneItem>();

            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
            {
                if (currentSceneItems != null)
                {
                    //清场
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }
                    foreach (var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrfabs, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);
                    }
                }

            }
        }
    }
}

