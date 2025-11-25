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

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
    }
    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
    }

    

    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite = normal;
        SetCursorImage(normal);
    }

    private void Update()
    {
        if (cursorImage == null)
            return;
        cursorImage.transform.position = Input.mousePosition;
        if(!InteractWithUI())
        {
            SetCursorImage(currentSprite);
        }
        else
        {
            SetCursorImage(normal);
        }
    }
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelcted)
    {
        if(!isSelcted)
        {
            currentSprite = normal;
        }
        else //物品被选中
        {
            //WORKFLOW:添加所有类型对应的图片
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commondity => item,
                ItemType.ChopTool => tool,
                _ => normal
            };
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
