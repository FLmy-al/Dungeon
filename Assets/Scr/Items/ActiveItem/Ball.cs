using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Ball : ActiveItem
{
    private int damage => 1;

    public override int ItemID => 00001;

    public override string itemName => "球";

    public override string itemDescription => "使用时对敌人造成伤害";

    protected override int[] x => new int[] { 0, 0, 1, 1 };

    protected override int[] y => new int[] { 0, 1, 0, 1 };

    protected override int occupy => 4;

    protected override int CD => 4;

    public override void ActiveAbility()
    {
        AttackController.Instance.targetEnemy.GetDamage(damage);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(AttackController.Instance.isFighting)
        {
            if (UseItem())
            {
                ActiveAbility();
                SetAlpha(currentCD, CD);
            }
        }
        
    }
}
