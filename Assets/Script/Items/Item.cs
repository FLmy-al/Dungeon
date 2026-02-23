using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,
    IDragHandler, IBeginDragHandler, IEndDragHandler,
    ICanvasRaycastFilter
{
    [SerializeField]private Image image;//物品图片
    public abstract int ItemID { get; }//物品编号
    public abstract string itemName { get; }//物品名称
    public abstract string itemDescription { get; }//物品描述

    //网格占用
    protected abstract int[] x { get; set; }
    protected abstract int[] y { get; set; }
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

    [Header("旋转配置")]
    [SerializeField] private float currentRotation = 0f; // 当前旋转角度（Z轴）
    private const float RotateStep = 90f; // 每次顺时针旋转90度

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
        // 确保旋转围绕物品中心（Pivot设为0.5,0.5，避免旋转偏移）
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        //初始化纹理和像素数据
        InitTextureData();
    }

    private void Update()
    {
        // 仅拖拽中、非战斗时响应右键
        if (MouseController.Instance.isOnDrag && MouseController.Instance.currentDragItem == this)
        {
            if (Input.GetMouseButtonDown(1))
            {
                RotateItemClockwise();
            }
        }
    }

    /// <summary>
    /// 物品顺时针旋转90度，并更新网格占用偏移
    /// </summary>
    private void RotateItemClockwise()
    {
        // 1. 顺时针旋转90度（UGUI的Z轴旋转：负值=顺时针，正值=逆时针）
        currentRotation -= RotateStep;
        currentRotation = Mathf.Round(currentRotation % 360); // 取模360，避免角度累积过大
        rectTransform.localEulerAngles = new Vector3(0, 0, currentRotation);

        // 2. 更新网格占用偏移（子类需重写此方法实现具体的偏移旋转）
        UpdateOccupyGridOnRotate();

        Debug.Log($"[{itemName}] 顺时针旋转90度，当前角度：{currentRotation}°");
    }

    /// <summary>
    /// 绕形状中心顺时针旋转90度后的占用偏移（与图片绕 pivot 中心旋转一致，避免绕(0,0)导致显示错位）
    /// 步骤：求形心 → 相对形心旋转 → 归一化到最小为(0,0)
    /// </summary>
    protected (int[] newX, int[] newY) ComputeRotatedOffsetsAroundCenter()
    {
        if (occupy <= 0 || x == null || y == null) return (null, null);
        float cx = 0f, cy = 0f;
        for (int i = 0; i < occupy; i++)
        {
            cx += GetXAtIndex(i);
            cy += GetYAtIndex(i);
        }
        cx /= occupy;
        cy /= occupy;

        int[] nx = new int[occupy];
        int[] ny = new int[occupy];
        int minNx = int.MaxValue, minNy = int.MaxValue;
        for (int i = 0; i < occupy; i++)
        {
            float dx = GetXAtIndex(i) - cx;
            float dy = GetYAtIndex(i) - cy;
            // 顺时针90度（X右Y下）：(dx,dy) -> (-dy, dx)
            nx[i] = Mathf.RoundToInt(cx - dy);
            ny[i] = Mathf.RoundToInt(cy + dx);
            if (nx[i] < minNx) minNx = nx[i];
            if (ny[i] < minNy) minNy = ny[i];
        }
        for (int i = 0; i < occupy; i++)
        {
            nx[i] -= minNx;
            ny[i] -= minNy;
        }
        return (nx, ny);
    }

    /// <summary>
    /// 实现旋转后网格偏移的更新逻辑（子类应使用绕中心旋转以与图片一致）
    /// </summary>
    protected abstract void UpdateOccupyGridOnRotate();

    /// <summary>
    /// 从另一个物品深拷贝占用偏移（放置时保证新实例的网格形状与传入物品一致，例如旋转后的状态）
    /// </summary>
    public abstract void CopyOccupancyFrom(Item other);

    //初始化纹理数据（关键）
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
            MouseController.Instance.currentDragItem = this;
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
        MouseController.Instance.currentDragItem = null;
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