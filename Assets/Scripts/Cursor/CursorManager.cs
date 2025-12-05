using SFarm.Map;
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
    public Image buildImage;
    private RectTransform cursorCanvas;

    private Camera mainCamera;
    private Grid currentGrid;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;
    private bool cursorPositionValid;

    private ItemDetails currentItem;
    private Transform playerTransform => FindObjectOfType<Player>().transform;
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
        if (!InteractWithUI() && cursorEnable) 
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
        }
    }
    private void CheckPlayerInput()
    {
        if(Input.GetMouseButtonDown(0)&&cursorPositionValid)
        {
            //执行方法
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }
    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }
    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }
    #region 设置鼠标图片
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
    /// 设置鼠标可用
    /// </summary>
    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
  
    }
    /// <summary>
    /// 设置鼠标不可用
    /// </summary>
    private void SetCursorInValid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
      
    }
    #endregion

    /// <summary>
    /// 物品选择事件函数
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelcted"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelcted)
    {
        
        if(!isSelcted)
        {
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
        }
        else //物品被选中才切换图片
        {
            currentItem = itemDetails;
            
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
            cursorEnable = true;
        }
        
    }
    private void CheckCursorValid()
    {
        // 转换鼠标屏幕坐标到世界坐标（z轴设为0，避免深度问题）
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

       
        TileDetails currentTile = GirdMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        var playerGridPos = currentGrid.WorldToCell(playerTransform.position);

        //判断在使用范围内
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }
        if (currentTile != null)
        {
            switch (currentItem.itemType)
            {
                case ItemType.Commodity:
                    if (currentTile.canDropItem) 
                        SetCursorValid();
                    else 
                        SetCursorInValid();
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig)
                    {
                        Debug.Log("2");
                        SetCursorValid();
                    }  
                    else 
                        SetCursorInValid();
                    break;
                 case ItemType.WaterTool:
                    //如果被挖且没有被浇水
                    if (currentTile.daysSinceDug > -1 && currentTile.daysSinceWatered == -1) SetCursorValid(); else SetCursorInValid();
                    break;
            }
        }
        else
        {
            SetCursorInValid();
        }
       
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
