using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EventManager : MonoBehaviour
{
    //单例模式
    public static EventManager Instance { get; private set; }
    //事件字典
    private Dictionary<EventTypes, Action<IEvent>> _eventBus = new Dictionary<EventTypes, Action<IEvent>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        //切换场景不销毁
        DontDestroyOnLoad(gameObject);

        Debug.Log("初始化事件监听器");
    }

    //注册事件监听
    public void RegisterEvent(EventTypes type, Action<IEvent> listener)
    {
        if (listener == null)
        {
            Debug.Log("尝试注册空的监听");
            return;
        }

        if (_eventBus.ContainsKey(type))//如果该事件已经添加过
        {
            //重复监听
            if (_eventBus[type].GetInvocationList().Contains(listener))
            {
                Debug.Log($"事件{type}已经添加过{listener}监听");
                return;
            }

            _eventBus[type] += listener;
        }
        else
        {
            Debug.Log($"事件{type}被{listener}注册监听");
            _eventBus.Add(type, listener);
        }
    }
    //注销事件监听
    public void UnregisterEvent(EventTypes type, Action<IEvent> listener)
    {
        if (listener == null)
        {
            Debug.Log("尝试注销空的监听");
            return;
        }

        if (_eventBus.ContainsKey(type))
        {
            if (_eventBus[type].GetInvocationList().Contains(listener))
            {
                _eventBus[type] -= listener;
            }
            else
            {
                Debug.Log($"{listener}未监听事件{type}");
                return;
            }

            // 如果该事件没有监听了，移除字典条目（优化内存）
            if (_eventBus[type] == null)
            {
                _eventBus.Remove(type);
            }
        }
        else
        {
            Debug.Log("要注销的事件还未被注册");
            return;
        }
    }
    //触发事件
    public void TriggerEvent(EventTypes eventType, IEvent _event)
    {
        // 检查事件类型是否有监听
        if (_eventBus.TryGetValue(eventType, out var listeners))
        {
            try
            {
                // 触发所有监听回调
                listeners?.Invoke(_event);
            }
            catch (Exception e)
            {
                Debug.LogError($"触发事件{eventType}时出错：{e.Message}\n{e.StackTrace}");
            }
        }
    }
    //清空注册事件
    public void ClearEvent()
    {
        _eventBus.Clear();
        Debug.Log("所有事件已清空");
    }
}
