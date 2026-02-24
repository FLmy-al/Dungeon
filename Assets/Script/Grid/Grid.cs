using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid : MonoBehaviour
{
    public bool isUsing;
    public int pos_x;
    public int pos_y;
    private GridController parent;
    private Item currentItem;

    public Item GetCurrentItem() => currentItem;
    public void SetCurrentItem(Item item) => currentItem = item;
    private void Awake()
    {
        isUsing = false;
    }


}
