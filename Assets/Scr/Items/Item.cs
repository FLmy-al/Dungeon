using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// 移除无用的命名空间引用
// using static UnityEngine.GraphicsBuffer;

public abstract class Item : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,
    IDragHandler, IBeginDragHandler, IEndDragHandler,
    ICanvasRaycastFilter // 新增：实现射线检测过滤接口
{
    private Image image;//物品图片
    public abstract int ItemID { get; }//物品编号
    public abstract string itemName { get; }//物品名称
    public abstract string itemDescription { get; }//物品描述

    //网格占用
    protected abstract int[] x { get; }
    protected abstract int[] y { get; }
    protected abstract int occupy { get; }//该物品占用格数

    //当前位置
    private GridController currentCanvas;//所在画布
    private Grid currentGrid;//所在网格(原点)
    private RectTransform canvasTransform;//画布的rectTransform
    private RectTransform rectTransform;//rectTransform

    // 新增：射线检测阈值（Alpha值大于此值才响应检测，避免浮点误差）
    [SerializeField] private float alphaThreshold = 0.1f;
    // 新增：缓存纹理像素数据，避免重复读取
    private Color[] pixelData;
    private Texture2D itemTexture;

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

    public Image GetImage() => image;

    protected void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        // 新增：初始化纹理和像素数据
        InitTextureData();
    }

    // 新增：初始化纹理数据（关键）
    private void InitTextureData()
    {
        if (image == null || image.sprite == null || image.sprite.texture == null)
        {
            Debug.LogWarning($"[{gameObject.name}] 物品图片或Sprite为空，无法启用像素级射线检测");
            return;
        }

        itemTexture = image.sprite.texture;
        // 检查纹理是否可读写（Unity中必须手动设置）
        if (!itemTexture.isReadable)
        {
            Debug.LogError($"[{gameObject.name}] 物品纹理{itemTexture.name}未开启Read/Write Enabled！请在Inspector中设置：Texture -> Import Settings -> Read/Write Enabled = true，然后点击Apply");
            return;
        }

        // 缓存纹理所有像素数据（避免每次检测都读取，提升性能）
        pixelData = itemTexture.GetPixels();
    }

    // 设置透明度
    public void SetAlpha(float current, float max)
    {
        float cdProgress = current / max;
        Color tempColor = image.color;
        tempColor.a = Mathf.Lerp(0.5f, 1f, cdProgress);
        image.color = tempColor;
    }

    //鼠标移动到该物品上
    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseController.Instance.onItem = this;
        if (!MouseController.Instance.isOnDrag)
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

    // 开始拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!AttackController.Instance.isFighting)
        {
            MouseController.Instance.isOnDrag = true;
            Debug.Log("开始拖拽");
            // 清理物品占用
            ClearItemFormGrids();
            // 移除物品列表
            currentCanvas.GetItems().Remove(this);
        }
        else
        {
            Debug.Log("战斗中，无法拖拽");
        }
    }

    // 拖拽中
    public void OnDrag(PointerEventData eventData)
    {
        if (!AttackController.Instance.isFighting)
        {
            // 关闭射线检测
            image.raycastTarget = false;
            // 计算鼠标位置
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

    // 拖拽结束时
    public void OnEndDrag(PointerEventData eventData)
    {
        MouseController.Instance.isOnDrag = false;
        // 开启射线检测
        image.raycastTarget = true;
        try
        {
            if (!AttackController.Instance.isFighting)
            {
                // 向目标位置添加物品
                if (MouseController.Instance.onGrid != null)
                {
                    // 获取目标位置单元格
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
                    // 添加物品
                    if (MouseController.Instance.onGrid.CheckCanAddItem(this, MouseController.Instance.onGrid.FindGrid(target_x, target_y)))
                    {
                        MouseController.Instance.onGrid.AddItemToTargetGrid(this, MouseController.Instance.onGrid.FindGrid(target_x, target_y));
                    }
                    else
                    {
                        ReturnToOriginalPosition();
                    }
                    Debug.Log("拖拽结束");
                    // 删除物品
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
            Debug.Log(message: ex.Message);
            ReturnToOriginalPosition();
        }
    }

    // 清理物品占用
    public void ClearItemFormGrids()
    {
        // 清理网格
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
            if (currentCanvas.CheckCanAddItem(this, currentGrid))
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

    // 核心实现：ICanvasRaycastFilter接口方法（判断射线检测是否有效）
    public bool IsRaycastLocationValid(Vector2 screenPosition, Camera eventCamera)
    {
        // 1. 基础校验：如果图片/纹理/像素数据为空，直接返回false（不响应检测）
        if (image == null || image.sprite == null || itemTexture == null || pixelData == null)
        {
            return false;
        }

        // 2. 将屏幕坐标转换为图片的本地坐标
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            screenPosition,
            eventCamera,
            out localPoint))
        {
            return false;
        }

        // 3. 将本地坐标转换为纹理的像素坐标（关键）
        Rect rect = image.sprite.rect;
        Vector2 pivot = rectTransform.pivot;
        Vector2 size = rectTransform.rect.size;

        // 计算点击位置在纹理中的相对位置（0~1范围）
        float x = (localPoint.x / size.x) + pivot.x;
        float y = (localPoint.y / size.y) + pivot.y;

        // 转换为像素坐标（确保在纹理范围内）
        int pixelX = Mathf.Clamp(Mathf.RoundToInt(rect.x + x * rect.width), 0, itemTexture.width - 1);
        int pixelY = Mathf.Clamp(Mathf.RoundToInt(rect.y + y * rect.height), 0, itemTexture.height - 1);

        // 4. 计算像素在数组中的索引（Texture2D的像素数组是按行存储的，原点在左下角）
        int pixelIndex = pixelY * itemTexture.width + pixelX;
        if (pixelIndex < 0 || pixelIndex >= pixelData.Length)
        {
            return false;
        }

        // 5. 判断像素Alpha值是否大于阈值（只有非透明区域才响应检测）
        float alpha = pixelData[pixelIndex].a * image.color.a; // 叠加图片自身的透明度
        return alpha > alphaThreshold;
    }
}