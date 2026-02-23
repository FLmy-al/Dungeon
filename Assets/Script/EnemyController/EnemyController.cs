using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance {  get; private set; }

    public List<Enemy> enemies = new List<Enemy>();

    public List<GameObject> enemySpawns = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent(EventTypes.FightStartEvent, SpawnEnemies);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnregisterEvent(EventTypes.FightStartEvent, SpawnEnemies);
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
    //从字典中随机取一组敌人生成
    public void SpawnEnemies(IEvent @event)
    {
        List<Enemy> enemies = EnemyPool.instance.EnemyGroupsDict[Random.Range(1, EnemyPool.instance.EnemyGroupsDict.Count)];
        for(int i = 0; i < enemies.Count; i++ )
        {
            // 确保生成点足够
            if (i >= enemySpawns.Count)
            {
                Debug.LogError("生成点不足，无法生成所有敌人！");
                break;
            }
            Enemy enemy = Instantiate(enemies[i], enemySpawns[i].transform.position,Quaternion.identity);
            this.enemies.Add(enemy);
        }
    }
}
