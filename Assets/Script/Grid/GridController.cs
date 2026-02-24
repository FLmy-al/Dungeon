using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class GridController : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public static int pixelSize => 80;//物品/网格大小

    protected abstract int x { get; }//网格宽度
    protected abstract int y { get; }//网格高度
    public MyGrid gridPrefab;//网格预制体索引
    private List<MyGrid> grids;//该控制器下的网格
    protected List<Item> items;//该网格中的物品

    public List<Item> GetItems() => items;

    private void Awake()
    {
        grids = new List<MyGrid>();
        items = new List<Item>();
        //初始化网格
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                MyGrid newgrid = Instantiate(gridPrefab, transform);
                newgrid.transform.localPosition = new Vector2(j * pixelSize, -i * pixelSize);
                newgrid.name = "grid_" + j + "_" + i;
                newgrid.GetComponent<MyGrid>().pos_x = j;
                newgrid.GetComponent<MyGrid>().pos_y = i;
                grids.Add(newgrid);
                Debug.Log("创建" + newgrid.name);
            }
        }  
    }

    private void Start()
    {
        //添加一个物品到背包
        if (!AddItemToGrid(ItemPool.Instance.items[0]))
        {
            Debug.Log("添加失败：没有空余位置");
        }
    }
    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseController.Instance.onGrid = this;
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        MouseController.Instance.onGrid = null;
    }
    //寻找网格
    public MyGrid FindGrid(int targetX, int targetY)
    {
        // 提前检查坐标是否超出网格范围（X=列，Y=行）
        if (targetX < 0 || targetX >= this.y || targetY < 0 || targetY >= this.x)
        {
            Debug.LogWarning($"网格坐标({targetX},{targetY})超出范围（宽={this.y}, 高={this.x}）");
            return null;
        }

        // 计算索引：行*列数 + 列
        int index = targetY * this.y + targetX;
        if (index >= 0 && index < grids.Count)
        {
            return grids[index];
        }
        return null;
    }
    //添加一个物品到网格
    public bool AddItemToGrid(Item item)
    {
        foreach (MyGrid grid in grids)
        {
            if (CheckCanAddItem(item, grid))
            {
                AddItemToTargetGrid(item, grid);
                return true;
            }
        }
        return false;
    }
    //检查是否可以放置
    public bool CheckCanAddItem(Item item, MyGrid originGrid)
    {
        MyGrid currentGrid = originGrid;
        for (int i = 0; i < item.GetOccupyCount(); i++)
        {
            // 计算当前格子的绝对坐标（原点+偏移）
            int targetX = originGrid.pos_x + item.GetXAtIndex(i);
            int targetY = originGrid.pos_y + item.GetYAtIndex(i);

            // 1. 找目标格子
            MyGrid targetGrid = FindGrid(targetX, targetY);
            if (targetGrid == null)
            {
                Debug.Log($"物品{item.itemName}的格子({targetX},{targetY})超出网格范围，无法放置");
                return false;
            }

            // 2. 检查格子是否被占用
            if (targetGrid.isUsing)
            {
                Debug.Log($"物品{item.itemName}的格子({targetX},{targetY})已被占用，无法放置");
                return false;
            }

        }
        Debug.Log("可以放置");
        return true;
    }

    //添加物品到指定网格（使用传入 item 的占用偏移，确保旋转后的格子正确）
    public void AddItemToTargetGrid(Item item, MyGrid originGrid)
    {
        Item newitem = Instantiate(item, transform);//创建Item
        newitem.name = item.name;
        newitem.CopyOccupancyFrom(item); // 同步旋转后的占用格子，保证网格与图片一致
        //给实例化组件赋值
        newitem.SetCurrentCanvas(this);
        newitem.SetCanvasTransform(GetComponent<RectTransform>());
        newitem.SetCurrentGrid(originGrid);
        //设置物品位置
        newitem.transform.localPosition = new Vector2(pixelSize * (originGrid.pos_x + newitem.GetRectTransform().rect.width / pixelSize / 2), -pixelSize * (originGrid.pos_y + newitem.GetRectTransform().rect.height / pixelSize / 2));
        // 使用传入 item 的占用偏移标记网格（保证旋转后的形状与图片一致，避免 Instantiate 浅拷贝导致引用混乱）
        for (int i = 0; i < item.GetOccupyCount(); i++)
        {
            int targetX = originGrid.pos_x + item.GetXAtIndex(i);
            int targetY = originGrid.pos_y + item.GetYAtIndex(i);

            MyGrid targetGrid = FindGrid(targetX, targetY);
            if (targetGrid != null)
            {
                targetGrid.isUsing = true;
                targetGrid.SetCurrentItem(newitem);
                Debug.Log($"标记格子({targetX},{targetY})为已使用，物品：{newitem.itemName}");
            }
        }
        items.Add(newitem);
    }
}
