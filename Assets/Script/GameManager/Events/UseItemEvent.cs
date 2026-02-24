using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemEvent : IEvent
{
    Item item;
    public UseItemEvent(Item _item)
    {
        item = _item;
    }

    public Item GetUseItem()
    {
        return item;
    }
}
