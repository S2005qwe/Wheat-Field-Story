using SFarm.CropPlant;

using UnityEngine;

namespace SFarm.Inventory
{
    public class Item : MonoBehaviour
    {
        public int itemID;  //物品ID

        private SpriteRenderer spriteRenderer; //物品图标

        public ItemDetails itemDetails; //物品数据

        private BoxCollider2D coll; //物品身上的碰撞器

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();
        }


        private void Start()
        {
            if(itemID != 0)
            {
                Init(itemID);
            }
        }

        //初始化
        public void Init(int ID)
        {
            itemID = ID;

            //Inventory获得当前数据
            itemDetails = InventoryManager.Instance.GetItemDetails(itemID);

            if(itemDetails !=null)
            {
                //将世界图标赋值给它，有则给它 没有则把icon给它
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite != null ? itemDetails.itemOnWorldSprite : itemDetails.itemIcon;    
                
                //修改碰撞体尺寸
                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x,spriteRenderer.sprite.bounds.size.y);
                coll.size = newSize;
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }
            if (itemDetails.itemType == ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ReapItem>();
                gameObject.GetComponent<ReapItem>().InitCropData(itemDetails.itemId);
                gameObject.AddComponent<ItemInteractive>();
            }

        }
    }
}

