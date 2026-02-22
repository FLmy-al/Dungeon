using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Ball : ActiveItem
{
    // 初始网格偏移（2x2横向：占用(0,0),(1,0),(0,1),(1,1)）
    private int[] _x = new int[] { 0, 1 ,0 ,1};
    private int[] _y = new int[] { 0, 0 ,1 ,1};
    private int damage => 1;

    public override int ItemID => 00001;

    public override string itemName => "球";

    public override string itemDescription => "使用时对敌人造成伤害";

    protected override int[] x { get => _x; set => _x = value; }

    protected override int[] y { get => _y; set => _y = value; }

    protected override int occupy => 4;

    protected override int CD => 4;

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
