using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boomerang : ActiveItem
{
    // 初始网格偏移（L型：占用(0,0),(1,0)),(1,1)）
    private int[] _x = new int[] { 0, 1 ,1};
    private int[] _y = new int[] { 0, 0 ,1};

    private int damage = 2;
    private int _CD = 3;
    public override int ItemID => 00002;

    public override string itemName => "回旋镖";

    public override string itemDescription => "使用时对敌人造成2点伤害";

    protected override int CD { get => _CD; set => _CD = value; }

    protected override int[] x { get => _x; set => _x = value; }

    protected override int[] y { get => _y; set => _y = value; }

    protected override int occupy => 3;

    public override void ActiveAbility()
    {
        AttackController.Instance.targetEnemy.GetDamage(damage);
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
