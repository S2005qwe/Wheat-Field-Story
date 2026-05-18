using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFarm.Inventory
{
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            //查看碰撞到的物体是否有Item脚本
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                //如果碰到的物体可以被拾取
                if (item.itemDetails.canPickedUp)
                {
                    //拾取物品添加到背包
                    InventoryManager.Instance.AddItem(item, true);

                    EventHandler.CallPlaySoundEvent(SoundName.Pickup);
                }
            }
        }
    }
}

