using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boomerang : ActiveItem
{
    private int damage = 2;
    public override int ItemID => 00002;

    public override string itemName => "回旋镖";

    public override string itemDescription => "使用时对敌人造成2点伤害";

    protected override int CD => 5;

    protected override int[] x => new int[] { 0,1,1};

    protected override int[] y => new int[] { 0,0,1};

    protected override int occupy => 3;

    public override void ActiveAbility()
    {
        AttackController.Instance.targetEnemy.GetDamage(damage);
    }
}
