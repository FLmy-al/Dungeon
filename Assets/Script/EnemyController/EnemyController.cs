using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance {  get; private set; }

    public List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        Instance = this;
    }

    public void ReduceEnemyAttackTime()
    {
        if(enemies.Count <= 0)
        {
            EventManager.Instance.TriggerEvent(EventTypes.FightEndEvent, new FightEndEvent());
        }

        foreach(Enemy enemy in enemies)
        {
            enemy.currentAttackTime--;
            
            if (enemy == null || enemy.currentHP <= 0)
            {
                continue;
            }

            //敌人行动
            if (enemy.currentAttackTime <= 0 && enemy.currentHP > 0)
            {
                enemy.Attack();
            }

            //更新敌人行动间隔
            if (enemy.currentAttackTime <= 0)
            {
                enemy.currentAttackTime = enemy.attackTime;
            }
            enemy.currentAttackTimeText.text = enemy.currentAttackTime.ToString();
        }
    }
}
