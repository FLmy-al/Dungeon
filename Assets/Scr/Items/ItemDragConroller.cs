using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ItemDragConroller : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    //网格占用
    public int[] x;
    public int[] y;
    public int occupy;//该物品占用格数

    //当前位置
    public GridController currentCanvas;//所在画布
    public Grid currentGrid;//所在网格(原点)
    private RectTransform canvasTransform;//画布的rectTransform
    private RectTransform rectTransform;//rectTransform

    private void Awake()
    {
        InitOccupy();
        rectTransform = GetComponent<RectTransform>();
    }
    //开始拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!AttackController.Instance.isFighting)
        {
            MouseController.Instance.isOnDrag = true;
            Debug.Log("开始拖拽");
            //清理物品占用
            ClearItemFormGrids();
        }else
        {
            Debug.Log("战斗中，无法拖拽");
        }
    }
    //拖拽中
    public void OnDrag(PointerEventData eventData)
    {
        if(!AttackController.Instance.isFighting)
        {
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
        if(!AttackController.Instance.isFighting)
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
                AddItemToGrids(MouseController.Instance.onGrid, MouseController.Instance.onGrid.FindGrid(target_x, target_y));
                Debug.Log("拖拽结束");
                //删除物品
                Debug.Log("删除物品：" + gameObject.name);
                MouseController.Instance.isOnDrag = false;
                Destroy(gameObject);
            }
            else
            {
                AddItemToGrids(currentCanvas, currentGrid);
                Debug.Log("返回原来位置");
                Destroy(gameObject);
            }
        }
    }

    //初始化物品占用
    public abstract void InitOccupy();
    //清理物品占用
    public void ClearItemFormGrids()
    {
        //清理背包列表
        currentCanvas.GetComponent<PackageItems>().items.Remove(GetComponent<Item>());
        //清理网格
        for(int i = 0; i < occupy;i++)
        {
            currentCanvas.FindGrid(currentGrid.pos_x + x[i], currentGrid.pos_y + y[i]).isUsing = false;
            Debug.Log("已清理" + currentGrid.pos_x + x[i] + "_" + currentGrid.pos_y + y[i]);
        }
    }
    //添加物品到
    public void AddItemToGrids(GridController gridController,Grid grid)
    {
        Item item = GetComponent<Item>(); 
        Item newitem = Instantiate(item, gridController.transform);//创建Item
        newitem.name = gameObject.name;
        ItemDragConroller newDragCtrl = newitem.GetComponent<ItemDragConroller>();//获取实例化的组件
        //给实例化组件赋值
        newDragCtrl.currentCanvas = gridController;
        newDragCtrl.canvasTransform = newDragCtrl.currentCanvas.GetComponent<RectTransform>();
        newDragCtrl.currentGrid = grid;
        //设置物品位置
        newitem.transform.localPosition = new Vector2(GridController.pixelSize * grid.pos_x, -GridController.pixelSize * grid.pos_y);
        //加入背包物品列表
        PackageItems.Instance.items.Add(newitem);
        //修改网格状态
        Grid changeGrid = grid;
        for (int i = 0; i < newitem.GetComponent<ItemDragConroller>().occupy; i++)
        {
            changeGrid = gridController.FindGrid(grid.pos_x + newitem.GetComponent<ItemDragConroller>().x[i], grid.pos_y + newitem.GetComponent<ItemDragConroller>().y[i]);
            changeGrid.isUsing = true;
        }
    }
}
