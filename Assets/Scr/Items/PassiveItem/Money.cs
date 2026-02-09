using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : PassiveItem
{
    public override int ItemID => 10001;

    public override string itemName => "钱";

    public override string itemDescription => "可以购买物品";

    protected override int[] x => new int[] { 0 };

    protected override int[] y => new int[] { 0 };

    protected override int occupy => 1;

    public override void PassiveAbility()
    {
        
    }
}
