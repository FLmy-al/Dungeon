using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridController : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public static int pixelSize;//物品/网格大小

    private int x;//网格宽度
    private int y;//网格高度
    public Grid gridPrefab;//网格预制体索引
    public List<Grid> grids;//该控制器下的网格

    private void Awake()
    {
        pixelSize = 80;
        x = 8;
        y = 8;

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
        if (CheckCanAddItem(grids[1], ItemPool.Instance.items[0]))
        {
            ItemPool.Instance.items[0].AddItemToGrids(this, grids[1]);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseController.Instance.onGrid = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseController.Instance.onGrid = null;
    }

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
    //检查是否可以放置
    public bool CheckCanAddItem(Grid grid,Item item)
    {
        Grid currentGrid = grid;
        for(int i = 0;i < item.GetOccupy();i++)
        {
            //如果网格被占用
            if(currentGrid.isUsing)
            {
                Debug.Log("被占用，无法放置");
                return false;
            }else
            {
                currentGrid = FindGrid(grid.pos_x + item.GetPos_X(i), grid.pos_y + item.GetPos_Y(i));
                //Debug.Log(grid.pos_x + item.GetComponent<ItemDragConroller>().x[i] + " " + grid.pos_y + item.GetComponent<ItemDragConroller>().y[i]);
            }
        }
        Debug.Log("可以放置");
        return true;
    }
}
