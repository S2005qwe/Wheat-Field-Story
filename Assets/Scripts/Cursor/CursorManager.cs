using SFram.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed,item;
    private Sprite currentSprite;
    private Image cursorImage;
    private RectTransform cursorCanvas;

    private Camera mainCamera;
    private Grid currentGird;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGirdPos;

    private bool curcorEnable;
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
      
    }
    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    

    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite = normal;
        SetCursorImage(normal);
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (cursorCanvas == null)
            return;
        cursorImage.transform.position = Input.mousePosition;
        if(!InteractWithUI()&& curcorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
        }
        else
        {
            SetCursorImage(normal);
        }
    }
    private void OnBeforeSceneUnloadEvent()
    {
        curcorEnable = false;
    }
    private void OnAfterSceneLoadedEvent()
    {
        currentGird = FindObjectOfType<Grid>();
        curcorEnable = true;
    }
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// 物品选择事件函数
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelcted"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelcted)
    {
        if(!isSelcted)
        {
            currentSprite = normal;
        }
        else //物品被选中才切换图片
        {
            //WORKFLOW:添加所有类型对应的图片
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                ItemType.CollectTool => tool,
                _ => normal
            };
        }
        
    }
    private void CheckCursorValid()
    {
        // 转换鼠标屏幕坐标到世界坐标（z轴设为0，避免深度问题）
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGirdPos = currentGird.WorldToCell(mouseWorldPos);

        Debug.Log("WorldPos:" + mouseWorldPos + "GridPos" + mouseGirdPos);
    }

    //是否与UI互动
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
