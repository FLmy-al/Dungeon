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
        EventManager.Instance.RegisterEvent(EventTypes.UseItemEvent, ReduceEnemyAttackTime);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnregisterEvent(EventTypes.FightStartEvent, SpawnEnemies);
        EventManager.Instance.UnregisterEvent(EventTypes.UseItemEvent, ReduceEnemyAttackTime);
    }

    public void ReduceEnemyAttackTime(IEvent @event)
    {
        foreach(Enemy enemy in enemies)
        {
            enemy.ReduceAttackTime();
            
            if (enemy == null || enemy.currentHP <= 0)
            {
                continue;
            }
        }
    }
    //从字典中随机取一组敌人生成
    public void SpawnEnemies(IEvent @event)
    {
        Debug.Log("生成敌人");
        List<Enemy> enemies = EnemyPool.instance.EnemyGroupsDict[Random.Range(1, EnemyPool.instance.EnemyGroupsDict.Count)];
        for(int i = 0; i < enemies.Count; i++ )
        {
            // 确保生成点足够
            if (i >= enemySpawns.Count)
            {
                Debug.LogError("生成点不足，无法生成所有敌人！");
                return;
            }
            Enemy enemy = Instantiate(enemies[i], enemySpawns[i].transform.position,Quaternion.identity);
            this.enemies.Add(enemy);
        }
    }
}
