using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {

        //管理数据
        public ItemDataList_SO itemDataList_SO;

        /// <summary>
        /// 通过ID返回物品信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        //查找数据（通过ID返回找到物品信息）
        public ItemDetails GetItemDetails(int ID)
        {
            //Find找item用于i代名，通过i.itemId->ID
            return itemDataList_SO.itemsDetailsList.Find(i => i.itemId == ID);
        }

        /// <summary>
        /// 添加物品到Player背包里
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory"></param>
        //添加物品
        public void AddItem(Item item,bool toDestory)
        {
            Debug.Log(GetItemDetails(item.itemID).itemId+"Name: "+GetItemDetails(item.itemID).itemName);
            
            //如果可以销毁，则销毁物品
            if (toDestory)
            {
                Destroy(item.gameObject);
            }
        }
    }
}

