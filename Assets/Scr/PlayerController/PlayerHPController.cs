using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPController : MonoBehaviour
{
    public static PlayerHPController Instance { get; private set; }

    public int playerMaxHP;//玩家最大血量
    public int playerCurrentHP;//玩家当前血量
    public Slider HPSlider;//玩家血量条
    public TMP_Text HP_Text;//玩家血量Text

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        playerCurrentHP = playerMaxHP;
        //初始化血量条
        HPSlider.maxValue = playerMaxHP;
        HPSlider.value = playerCurrentHP;
        HP_Text.text = playerCurrentHP.ToString() + "/" + playerMaxHP.ToString();
    }

    public void GetDamage(int damage)
    {
        playerCurrentHP -= damage;
        //更新血量条
        HPSlider.value = playerCurrentHP;
        HP_Text.text = playerCurrentHP.ToString() + "/" + playerMaxHP.ToString();

        if (playerCurrentHP <= 0)
        {
            //失败
        }
    }
}
