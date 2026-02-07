using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Image image;//物品图片

    [Header("物品详情")]
    public string itemName;//物品名称
    public string itemDescription;//物品描述

    protected void Start()
    {
        image = GetComponent<Image>();
    }

    //鼠标移动到该物品上
    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseController.Instance.onItem = this;
        if(!MouseController.Instance.isOnDrag)
        {
            ItemDetailUI.Instance.canvasGroup.alpha = 1f;
            ItemDetailUI.Instance.ShowItemDetail(this);
        }
    }
    //鼠标离开该物品时
    public void OnPointerExit(PointerEventData eventData)
    {
        ItemDetailUI.Instance.canvasGroup.alpha = 0f;
        MouseController.Instance.onItem = null;
    }

    //设置透明度
    public void SetAlpha(float current,float max)
    {
        float cdProgress = current / max;
        Color tempColor = image.color;
        tempColor.a = Mathf.Lerp(0.5f, 1f, cdProgress);
        image.color = tempColor;
    }
}
