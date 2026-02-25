using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackController : MonoBehaviour
{
    public static AttackController Instance { get; private set; }//单例模式

    public Enemy targetEnemy;
    public TargetSprite targetSprite;
    private TargetSprite currentTargetSprite;
    public bool isFighting;
    public Button StartFightButton;
    [SerializeReference]private DroppedGridController droppedGridController;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        isFighting = false;
    }

    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent(EventTypes.FightStartEvent, EnterAttackNode);//注册监听战斗开始事件
        EventManager.Instance.RegisterEvent(EventTypes.FightEndEvent, AttackEnd);//注册监听战斗结束事件
        EventManager.Instance.RegisterEvent(EventTypes.ExitFightNode, ExitAttackNode);//注册监听离开战斗节点
    }

    private void OnDisable()
    {
        EventManager.Instance.UnregisterEvent(EventTypes.FightStartEvent, EnterAttackNode);//取消监听战斗开始事件
        EventManager.Instance.UnregisterEvent(EventTypes.FightEndEvent, AttackEnd);//取消监听战斗结束事件
        EventManager.Instance.UnregisterEvent(EventTypes.ExitFightNode, ExitAttackNode);//取消监听离开战斗节点
    }

    public void ChooseEnemy(Enemy enemy)
    {
        targetEnemy = enemy;
        currentTargetSprite.transform.position = targetEnemy.transform.position;
    }

    //设置默认目标敌人
    public void DefaultTargetEnemy()
    {
        targetEnemy = EnemyController.Instance.enemies[0];
        currentTargetSprite = Instantiate(targetSprite,targetEnemy.transform);
    }

    //进入战斗节点
    public void EnterAttackNode(IEvent @event)
    {
        isFighting = true;
        DefaultTargetEnemy();
    }
    //战斗结束
    public void AttackEnd(IEvent @event)
    {
        isFighting = false;
        droppedGridController.gameObject.SetActive(true);
        droppedGridController.SpawnItems();
    }
    //退出战斗节点
    private void ExitAttackNode(IEvent @event)
    {
        isFighting = false;
        StartFightButton.gameObject.SetActive(true);
    }
}
