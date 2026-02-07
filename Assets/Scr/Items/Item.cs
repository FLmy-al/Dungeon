using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Image image;//物品图片

    [Header("物品详情")]
    public string itemName;//物品名称
    public string itemDescription;//物品描述

    //网格占用
    protected int[] x;
    protected int[] y;
    protected int occupy;//该物品占用格数

    //当前位置
    private GridController currentCanvas;//所在画布
    private Grid currentGrid;//所在网格(原点)
    private RectTransform canvasTransform;//画布的rectTransform
    private RectTransform rectTransform;//rectTransform

    protected void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }
    protected void Start()
    {
        
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
                //开启射线检测
                image.raycastTarget = true;
                //添加物品
                AddItemToGrids(MouseController.Instance.onGrid, MouseController.Instance.onGrid.FindGrid(target_x, target_y));
                Debug.Log("拖拽结束");
                //删除物品
                Debug.Log("删除物品：" + gameObject.name);
                MouseController.Instance.isOnDrag = false;
                Destroy(gameObject);
            }
            else
            {
                //开启射线检测
                image.raycastTarget = true;
                AddItemToGrids(currentCanvas, currentGrid);
                Debug.Log("返回原来位置");
                Destroy(gameObject);
            }
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
    //添加物品到
    public void AddItemToGrids(GridController gridController, Grid grid)
    {
        Item newitem = Instantiate(this, gridController.transform);//创建Item
        newitem.name = gameObject.name;
        //给实例化组件赋值
        newitem.currentCanvas = gridController;
        newitem.canvasTransform = newitem.currentCanvas.GetComponent<RectTransform>();
        newitem.currentGrid = grid;
        //设置物品位置
        newitem.transform.localPosition = new Vector2(GridController.pixelSize * (grid.pos_x + newitem.rectTransform.rect.width / 80 / 2), -GridController.pixelSize * (grid.pos_y + newitem.rectTransform.rect.height / 80 / 2));
        //加入背包物品列表
        PackageItems.Instance.items.Add(newitem);
        //修改网格状态
        Grid changeGrid = grid;
        for (int i = 0; i < newitem.occupy; i++)
        {
            changeGrid = gridController.FindGrid(grid.pos_x + newitem.x[i], grid.pos_y + newitem.y[i]);
            changeGrid.isUsing = true;
        }
    }

    public int GetOccupy()
    {
        return occupy;
    }

    public int GetPos_X(int index)
    {
        return x[index];
    }

    public int GetPos_Y(int index)
    {
        return y[index];
    }
}
