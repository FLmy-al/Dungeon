using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEditor.Progress;

public class PackageManager : GridController
{
    protected override int x => 8;
    protected override int y => 8;

    private void Start()
    {
        EventManager.Instance.RegisterEvent(EventTypes.UseItemEvent, ReduceItemCD);
        EventManager.Instance.RegisterEvent(EventTypes.FightEndEvent, ResetItemsCD);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnregisterEvent(EventTypes.UseItemEvent, ReduceItemCD);
        EventManager.Instance.UnregisterEvent(EventTypes.FightEndEvent, ResetItemsCD);
    }

    //减少除使用物品外的物品cd
    public void ReduceItemCD(IEvent @event)
    {
        UseItemEvent _event = (@event as UseItemEvent);
        foreach(Item item in items)
        {
            if(item != _event.GetUseItem() && item is ActiveItem)
            {
                ActiveItem _item = item as ActiveItem;
                _item.SetItemCurrentCD(_item.GetItemCurrentCD() + 1);
                Debug.Log($"物品{item.name}的当前cd为{_item.GetItemCurrentCD()}");
            }
        }
    }
    //重置所有物品CD
    public void ResetItemsCD(IEvent @event)
    {
        UseItemEvent _event = (@event as UseItemEvent);
        foreach (Item item in items)
        {
            if(item is ActiveItem)
            {
                ActiveItem _item = item as ActiveItem;
                _item.SetItemCurrentCD(_item.GetItemCD());
                Debug.Log($"物品{item.name}的当前cd为{_item.GetItemCurrentCD()}");
            }
        }
    }
}
