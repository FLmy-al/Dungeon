using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedGridController : GridController
{
    protected override int x => 12;

    protected override int y => 12;

    //清除网格内物品
    public void ClearItems()
    {
        foreach(Item _item in items)
        {
            Destroy(_item);
        }
    }
    //生成物品
    public void SpawnItems()
    {
        for(int i = 0; i <= Random.Range(0, 5); i++)
        {
            Item _item = ItemPool.Instance.items[Random.Range(0,ItemPool.Instance.items.Count)];
            AddItemToGrid(_item);
        }
    }
}
