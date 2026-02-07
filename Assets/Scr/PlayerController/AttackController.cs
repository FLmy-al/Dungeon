using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public static AttackController Instance { get; private set; }//单例模式

    public Enemy targetEnemy;
    public TargetSprite targetSprite;
    private TargetSprite currentTargetSprite;
    public bool isFighting;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        EnterAttackNode();
        isFighting = true;
    }

    public void ChooseEnemy(Enemy enemy)
    {
        Destroy(currentTargetSprite.gameObject);
        targetEnemy = enemy;
        currentTargetSprite = Instantiate(targetSprite, targetEnemy.transform);
    }

    //设置默认目标敌人
    public void DefaultTargetEnemy()
    {
        targetEnemy = EnemyController.Instance.enemies[0];
        currentTargetSprite = Instantiate(targetSprite,targetEnemy.transform);
    }

    //进入战斗节点
    private void EnterAttackNode()
    {
        isFighting = true;
        DefaultTargetEnemy();
    }

    //退出战斗节点
    private void ExitAttackNode()
    {

    }
}
