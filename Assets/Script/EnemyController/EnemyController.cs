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
        Debug.Log("2222");
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("1111");
    }

    private void OnEnable()
    {
        Debug.Log("3333");
        EventManager.Instance.RegisterEvent(EventTypes.FightStartEvent, SpawnEnemies);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnregisterEvent(EventTypes.FightStartEvent, SpawnEnemies);
    }

    public void ReduceEnemyAttackTime()
    {
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
