using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public abstract class Item : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Image image;//物品图片
    public abstract int ItemID { get; }//物品编号
    public abstract string itemName { get; }//物品名称
    public abstract string itemDescription { get; }//物品描述

    //网格占用
    protected int[] x;
    protected int[] y;
    protected int occupy;//该物品占用格数

    //当前位置
    private GridController currentCanvas;//所在画布
    private Grid currentGrid;//所在网格(原点)
    private RectTransform canvasTransform;//画布的rectTransform
    private RectTransform rectTransform;//rectTransform

    // 1. 网格占用相关
    public int GetOccupyCount() => occupy; // 获取占用格子数（替代原GetOccupy，命名更清晰）
    public int GetXAtIndex(int index) => x != null && index >= 0 && index < x.Length ? x[index] : 0; // 获取指定索引的X偏移
    public int GetYAtIndex(int index) => y != null && index >= 0 && index < y.Length ? y[index] : 0; // 获取指定索引的Y偏移

    // 2. 当前位置相关
    public GridController GetCurrentCanvas() => currentCanvas; // 获取所在GridController
    public Grid GetCurrentGrid() => currentGrid; // 获取所在网格原点
    public RectTransform GetRectTransform() => rectTransform; // 获取自身RectTransform

    // 3. Setter方法（用于外部赋值，比如GridController中设置物品位置）
    public void SetCurrentCanvas(GridController canvas) => currentCanvas = canvas;
    public void SetCurrentGrid(Grid grid) => currentGrid = grid;
    public void SetCanvasTransform(RectTransform transform) => canvasTransform = transform;

    protected void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    //鼠标移动到该物品上
    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseController.Instance.onItem = this;
        if(!MouseController.Instance.isOnDrag)
        {
            ItemDetailUI.Instance.canvasGroup.alpha = 1f;
            ItemDetailUI.Instance.ShowItemDetail(this);
        }
    }
    //鼠标离开该物品时
    public void OnPointerExit(PointerEventData eventData)
    {
        ItemDetailUI.Instance.canvasGroup.alpha = 0f;
        MouseController.Instance.onItem = null;
    }

    //设置透明度
    public void SetAlpha(float current,float max)
    {
        float cdProgress = current / max;
        Color tempColor = image.color;
        tempColor.a = Mathf.Lerp(0.5f, 1f, cdProgress);
        image.color = tempColor;
    }

    //开始拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!AttackController.Instance.isFighting)
        {
            MouseController.Instance.isOnDrag = true;
            Debug.Log("开始拖拽");
            //清理物品占用
            ClearItemFormGrids();
        }
        else
        {
            Debug.Log("战斗中，无法拖拽");
        }
    }
    //拖拽中
    public void OnDrag(PointerEventData eventData)
    {
        if (!AttackController.Instance.isFighting)
        {
            //关闭射线检测
            image.raycastTarget = false;
            //计算鼠标位置
            Vector2 MouseTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasTransform,
                    eventData.position,
                    Camera.main,
                    out MouseTransform
                    );
            rectTransform.localPosition = MouseTransform;
        }
    }
    //拖拽结束时
    public void OnEndDrag(PointerEventData eventData)
    {
        MouseController.Instance.isOnDrag = false;
        //开启射线检测
        image.raycastTarget = true;
        try
        {
            if (!AttackController.Instance.isFighting)
            {
                //向目标位置添加物品
                if (MouseController.Instance.onGrid != null)
                {
                    //获取目标位置单元格
                    canvasTransform = MouseController.Instance.onGrid.GetComponent<RectTransform>();
                    Vector2 gridTransform;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasTransform,
                        eventData.position,
                        Camera.main,
                        out gridTransform
                        );
                    int target_x = (int)gridTransform.x / GridController.pixelSize;
                    int target_y = -(int)gridTransform.y / GridController.pixelSize;
                    Debug.Log(gridTransform.x.ToString() + " " + gridTransform.y.ToString());
                    //添加物品
                    if(MouseController.Instance.onGrid.CheckCanAddItem(this, MouseController.Instance.onGrid.FindGrid(target_x, target_y)))
                    {
                        MouseController.Instance.onGrid.AddItemToTargetGrid(this, MouseController.Instance.onGrid.FindGrid(target_x, target_y));
                    }
                    Debug.Log("拖拽结束");
                    //删除物品
                    Debug.Log("删除物品：" + gameObject.name);
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    ReturnToOriginalPosition();
                    return;
                }
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.Log(message:  ex.Message);
            ReturnToOriginalPosition();
        }
    }
    //清理物品占用
    public void ClearItemFormGrids()
    {
        //清理背包列表
        currentCanvas.GetComponent<PackageItems>().items.Remove(GetComponent<Item>());
        //清理网格
        for (int i = 0; i < occupy; i++)
        {
            currentCanvas.FindGrid(currentGrid.pos_x + x[i], currentGrid.pos_y + y[i]).isUsing = false;
            Debug.Log("已清理" + currentGrid.pos_x + x[i] + "_" + currentGrid.pos_y + y[i]);
        }
    }

    private void ReturnToOriginalPosition()
    {
        if (currentCanvas != null && currentGrid != null)
        {
            if (currentCanvas.CheckCanAddItem(this,currentGrid))
            {
                currentCanvas.AddItemToTargetGrid(this, currentGrid);
            }
            Debug.Log("物品返回原位置");
        }
        else
        {
            Debug.LogWarning("无法返回原位置：currentCanvas或currentGrid为null");
        }

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
