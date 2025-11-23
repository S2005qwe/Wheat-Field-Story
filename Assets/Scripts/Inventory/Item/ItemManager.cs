using SFarm.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrfabs;

        private Transform itemParent;

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

        private void OnAfterSceneLoadedEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
        }


        private void OnInstaniateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPrfabs,pos,Quaternion.identity,itemParent);
            item.itemID = ID;
        }
    }
}

