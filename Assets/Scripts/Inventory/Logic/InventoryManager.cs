using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

namespace SFarm.Inventory
{
    /// <summary>
    /// 背包管理系统
    /// </summary>
    public class InventoryManager : Singleton<InventoryManager>
    {

        //管理数据
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;

        [Header("背包数据")]
        public InventoryBag_SO PlayerBag;

        [Header("交易")]
        public int playerMoney;
        

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
        }
        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
        }

        private void OnHarvestAtPlayerPosition(int ID)
        {
            //是否已经有该物品
            var index = GetItemIndexInBag(ID);
            AddItemAtIndex(ID, index, 1);
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, PlayerBag.itemList);
        }

        private void Start()
        {
            //更新背包UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, PlayerBag.itemList);
        }

        private void OnDropItemEvent(int ID, Vector3 pos, ItemType type)
        {
            ReMoveItem(ID, 1);
        }

        /// <summary>
        /// 查找数据（通过ID返回找到物品全部信息）
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            //Find找item用于i代名，通过i.itemId->ID，返回物品数据的列表中第i个
            return itemDataList_SO.itemsDetailsList.Find(i => i.itemId == ID);
        }



        /// <summary>
        /// 添加物品到Player背包里
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory"></param>

        public void AddItem(Item item, bool toDestory)
        {
            //是否已经有该物品
            var index = GetItemIndexInBag(item.itemID);
            AddItemAtIndex(item.itemID, index, 1);

            Debug.Log(GetItemDetails(item.itemID).itemId + "Name: " + GetItemDetails(item.itemID).itemName);

            //如果可以销毁，则销毁物品
            if (toDestory)
            {
                Destroy(item.gameObject);
            }

            //更新背包UI 
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, PlayerBag.itemList);
        }


        /// <summary>
        /// 检查背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < PlayerBag.itemList.Count; i++)
            {
                if (PlayerBag.itemList[i].itemID == 0)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// 通过物品ID找到背包已有物品位置
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>1则没有这个物品否则返回序号</returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < PlayerBag.itemList.Count; i++)
            {
                if (PlayerBag.itemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 在指定背包序号位置添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">序号</param>
        /// <param name="amount">数据</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            //通过索引来看，如果背包中没有这个物品,同时有空位
            if (index == -1 && CheckBagCapacity())
            {
                //则创建一个物品
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                //创建物品后，查找空位
                for (int i = 0; i < PlayerBag.itemList.Count; i++)
                {
                    if (PlayerBag.itemList[i].itemID == 0)
                    {
                        PlayerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else  //背包里有这个东西
            {
                int currentAmount = PlayerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
                PlayerBag.itemList[index] = item;
            }
        }


        /// <summary>
        /// Player背包范围内交换物品
        /// </summary>
        /// <param name="fromIndex">起始序号</param>
        /// <param name="toIndex">目标数据序号</param>
        public void SwapItem(int fromIndex, int toIndex)
        {
            InventoryItem currentItem = PlayerBag.itemList[fromIndex];
            InventoryItem targetItem = PlayerBag.itemList[toIndex];

            if (targetItem.itemID != 0)
            {
                PlayerBag.itemList[fromIndex] = targetItem;
                PlayerBag.itemList[toIndex] = currentItem;
            }
            else
            {
                PlayerBag.itemList[toIndex] = currentItem;
                PlayerBag.itemList[fromIndex] = new InventoryItem();
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, PlayerBag.itemList);
        }
        private void ReMoveItem(int ID, int removeAmount)
        {
            var index = GetItemIndexInBag(ID);

            if (PlayerBag.itemList[index].itemAmount > removeAmount)
            {
                var amount = PlayerBag.itemList[index].itemAmount - removeAmount;
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                PlayerBag.itemList[index] = item;
            }
            else if (PlayerBag.itemList[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                PlayerBag.itemList[index] = item;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, PlayerBag.itemList);
        }
       public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            //获得背包物品位置
            int index = GetItemIndexInBag(itemDetails.itemId);

            if (isSellTrade)     //卖
            {
                if (PlayerBag.itemList[index].itemAmount >= amount)
                {
                    ReMoveItem(itemDetails.itemId, amount);
                    cost = (int)(cost * itemDetails.sellPercentage);
                    playerMoney += cost;
                }
            }
            else if (playerMoney - cost >= 0)    //买
            {
                if(CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.itemId, index, amount);
                }
                playerMoney -= cost;
            }
            //刷新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, PlayerBag.itemList);
        }
    }

}

