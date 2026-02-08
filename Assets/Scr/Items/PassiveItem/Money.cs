using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : PassiveItem
{
    public override int ItemID => 10001;

    public override string itemName => "钱";

    public override string itemDescription => "可以购买物品";

    public override void InitOccupy()
    {
        x = new int[] { 0 };
        y = new int[] { 0 };
        occupy = 1;
    }

    public override void PassiveAbility()
    {
        
    }
}
