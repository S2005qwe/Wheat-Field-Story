using SFarm.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrfabs;  //物品预制体

        private Transform itemParent; //用于物品放在父物体下

        private void OnEnable()
        {
            EventHandler.InstaniateItemInScene += OnInstaniateItemInScene;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;

        }
        private void OnDisable()
        {
            EventHandler.InstaniateItemInScene -= OnInstaniateItemInScene;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        }


        //加载场景后寻找父物体
        private void OnAfterSceneLoadedEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
        }


        //示例化物品到场景中
        private void OnInstaniateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPrfabs,pos,Quaternion.identity,itemParent);
            item.itemID = ID;
        }
    }
}

