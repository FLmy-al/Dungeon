using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class GridController : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public static int pixelSize => 80;//物品/网格大小

    protected abstract int x { get; }//网格宽度
    protected abstract int y { get; }//网格高度
    public Grid gridPrefab;//网格预制体索引
    private List<Grid> grids;//该控制器下的网格

    private void Awake()
    {
        grids = new List<Grid>();
        //初始化网格
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                Grid newgrid = Instantiate(gridPrefab, transform);
                newgrid.transform.localPosition = new Vector2(i * pixelSize, -j * pixelSize);
                newgrid.name = "grid_" + i + "_" + j;
                newgrid.GetComponent<Grid>().pos_x = i;
                newgrid.GetComponent<Grid>().pos_y = j;
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
    public Grid FindGrid(int x, int y)
    {
        foreach(Grid grid in grids)
        {
            if(grid.pos_x == x && grid.pos_y == y)
            {
                return grid;
            }
        }
        return null;
    }
    //添加一个物品到网格
    public bool AddItemToGrid(Item item)
    {
        try
        {
            foreach (Grid grid in grids)
            {
                if(CheckCanAddItem(item,grid))
                {
                    AddItemToTargetGrid(item,grid);
                    return true;
                }
            }
            return false;
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError($"数组格子越界：{ex.Message}"); // 输出：Index was outside the bounds of the array.
            return false;
        }
    }
    //检查是否可以放置
    public bool CheckCanAddItem(Item item,Grid grid)
    {
        Grid currentGrid = grid;
        for (int i = 0; i < item.GetOccupyCount(); i++)
        {
            //如果网格被占用
            if (currentGrid.isUsing)
            {
                Debug.Log("被占用，无法放置");
                return false;
            }
            else
            {
                currentGrid = FindGrid(currentGrid.pos_x + item.GetXAtIndex(i), currentGrid.pos_y + item.GetYAtIndex(i));
                //Debug.Log(grid.pos_x + item.GetComponent<ItemDragConroller>().x[i] + " " + grid.pos_y + item.GetComponent<ItemDragConroller>().y[i]);
            }
        }
        Debug.Log("可以放置");
        return true;
    }

    //添加物品到指定网格
    public void AddItemToTargetGrid(Item item, Grid grid)
    {
        Item newitem = Instantiate(item, transform);//创建Item
        newitem.name = gameObject.name;
        //给实例化组件赋值
        newitem.SetCurrentCanvas(this);
        newitem.SetCanvasTransform(GetComponent<RectTransform>());
        newitem.SetCurrentGrid(grid);
        //设置物品位置
        newitem.transform.localPosition = new Vector2(pixelSize * (grid.pos_x + newitem.GetRectTransform().rect.width / 80 / 2), -pixelSize * (grid.pos_y + newitem.GetRectTransform().rect.height / 80 / 2));
        //加入背包物品列表
        PackageItems.Instance.items.Add(newitem);
        //修改网格状态
        Grid changeGrid = grid;
        for (int i = 0; i < newitem.GetOccupyCount(); i++)
        {
            changeGrid = FindGrid(grid.pos_x + newitem.GetXAtIndex(i), grid.pos_y + newitem.GetXAtIndex(i));
            changeGrid.isUsing = true;
        }
    }
}
