using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEditor : EditorWindow
{
    //ScriptableObject
    private ItemDataList_SO dataBase;

    //物品详情
    private List<ItemDetails> itemList = new List<ItemDetails>();

    //
    private VisualTreeAsset itemRowTemplate;

    //
    private ScrollView itemDetailsSection;

    //物品详情激活情况
    private ItemDetails activeItem;

    //默认预览图片
    private Sprite defaultIcon;

    private ListView itemListView;

    private VisualElement iconPreview;


    [MenuItem("S STUDIO/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }


    //创建GUI
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        //拿到模板数据
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/Item Row Template.uxml");

        //拿默认Icon图片
        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_M.png");

        //变量赋值
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = itemDetailsSection.Q<VisualElement>("Icon");

        //加载数据
        LoadDataBase();

        //生成ListView
        GenerteListView();
    }




    //获取数据
    private void LoadDataBase()
    {
        var dataArray = AssetDatabase.FindAssets("ItemDataList_SO");

        if (dataArray.Length > 1)
        {
            var path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            dataBase = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO))as ItemDataList_SO;
        }

        itemList = dataBase.itemsDetailsList;
        //如果不标记则无法保存数据
        EditorUtility.SetDirty(dataBase);
        //Debug.Log(itemList[0].itemId);


    }

    //生成列表
    private void GenerteListView()
    {
        Func<VisualElement> makeItem = ()=>itemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemList.Count) 
            {
                if (itemList[i].itemIcon!=null)
                e.Q<VisualElement>("Icon").style.backgroundImage = itemList[i].itemIcon.texture;
                e.Q<Label>("Name").text = itemList[i] ==null?"No ITEM" : itemList[i].itemName;
            }
        };

        
        itemListView.fixedItemHeight = 60;
        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;

        //当选择物品时调用函数方法
        itemListView.selectionChanged += OnListSelectionChange;

        //右侧信息面板不可见
        itemDetailsSection.visible = false;
    }

    //选择物品
    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        activeItem = (ItemDetails)selectedItem.First();
        GetItemDetails();
        itemDetailsSection.visible = true;
    }


    //获取物品信息(UI的右侧部分)
    private void GetItemDetails()
    {
        itemDetailsSection.MarkDirtyRepaint();

        //ID
        itemDetailsSection.Q<IntegerField>("ItemID").value = activeItem.itemId;
        itemDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemId = evt.newValue;
        });
        //Name
        itemDetailsSection.Q<TextField>("ItemName").value = activeItem.itemName;
        itemDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemName = evt.newValue;
            itemListView.Rebuild();
        });

        //Icon
        iconPreview.style.backgroundImage = activeItem.itemIcon.texture == null ? defaultIcon.texture : activeItem.itemIcon.texture;
        itemDetailsSection.Q<ObjectField>("ItemIcon").value = activeItem.itemIcon;
        itemDetailsSection.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;
            activeItem.itemIcon = newIcon;

            iconPreview.style.backgroundImage = newIcon==null?defaultIcon.texture:newIcon.texture;
            itemListView.Rebuild();
        });
    }

}
