using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : PassiveItem
{
    // 初始网格偏移（1x1：占用(0,0)）
    private int[] _x = new int[] { 0};
    private int[] _y = new int[] { 0};
    public override int ItemID => 10001;

    public override string itemName => "钱";

    public override string itemDescription => "可以购买物品";

    protected override int[] x { get => _x; set => _x = value; }

    protected override int[] y { get => _y; set => _y = value; }

    protected override int occupy => 1;

    public override void PassiveAbility()
    {
        
    }

    protected override void UpdateOccupyGridOnRotate()
    {
        var (newX, newY) = ComputeRotatedOffsetsAroundCenter();
        if (newX != null && newY != null)
        {
            _x = newX;
            _y = newY;
        }
    }

    public override void CopyOccupancyFrom(Item other)
    {
        if (other == null || other.GetOccupyCount() != occupy) return;
        for (int i = 0; i < occupy; i++)
        {
            _x[i] = other.GetXAtIndex(i);
            _y[i] = other.GetYAtIndex(i);
        }
    }
}
