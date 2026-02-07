using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHP;//敌人最大血量
    public int currentHP;//敌人当前血量
    public int attackTime;//攻击间隔
    public int currentAttackTime;//剩余攻击间隔
    public Slider HPSlider;//血量条
    public TMP_Text HP_Text;//血量Text
    public bool isAlive;//存活状态
    public TMP_Text currentAttackTimeText;//剩余攻击间隔Text

    private void Start()
    {
        currentHP = maxHP;
        currentAttackTime = attackTime;
        HPSlider.maxValue = maxHP;
        HPSlider.value = currentHP;
        HP_Text.text = currentHP.ToString() + "/" + maxHP.ToString();

        EnemyController.Instance.enemies.Add(this);//加入列表
        isAlive = true;
    }

    //点击事件
    private void OnMouseDown()
    {
        if(!isAlive)
        {
            return;
        }

        AttackController.Instance.ChooseEnemy(this); //攻击目标切换

        Debug.Log("当前目标为：" + name);
    }

    public void GetDamage(int damage)
    {
        if (!isAlive)
        {
            return;
        }

        currentHP -= damage;
        HPSlider.value = currentHP;
        HP_Text.text = currentHP.ToString() + "/" + maxHP.ToString();
        Debug.Log("当前目标血量为：" + currentHP);

        if (currentHP <= 0)
        {
            isAlive = false;
            EnemyController.Instance.enemies.Remove(this);//移出列表
            if(EnemyController.Instance.enemies.Count > 0)
            {
                AttackController.Instance.DefaultTargetEnemy();//切换目标敌人
            }
            Destroy(gameObject,0.1f);
        }
    }

    public virtual void Attack()
    {
        if (!isAlive)
        {
            return;
        }

        Debug.Log("EnemyAttack");
        //敌人攻击效果
        PlayerHPController.Instance.GetDamage(5);
    }
}
